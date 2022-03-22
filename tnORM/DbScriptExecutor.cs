using System.Data;
using System.Text.RegularExpressions;
using tnORM.Shared;

namespace tnORM
{
    public static class DbScriptExecutor
    {
        private const string GuidMonikerPrefix = "GUID";
        private static string UpgradeTableName { get; set; } = tnORMShared.UpgradeTableName;
        private static string UpgradeErrorTableName { get; set; } = tnORMShared.UpgradeErrorTableName;
        private static string Database { get; set; }
        private static string UpgradeScriptDirectory { get; set; }
        private static List<string> SqlScripts { get; set; } = new();
        

        public static void Run(string database)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException(nameof(database));
            }
            ConsoleLogger.LogLine("Initializing database script executor");
            Database = database;
            UpgradeScriptDirectory = tnORMConfig.GetString("ScriptSourceDirectory");
            ConsoleLogger.LogLine("Running database scripts");
            CheckForDatabase();
            CheckDbForUpgradeTables();
            GetScriptsToRun();
            RunUpgradeScripts();
        }


        private static void CheckForDatabase()
        {
            string query = $"SELECT COUNT(*) FROM sys.databases WHERE [Name] = '{Database}'";
            var result = QueryHandler.GetQueryResults(query);
            if ((int)result.Rows[0].ItemArray[0] == 0)
            {
                ConsoleLogger.LogLine($"Creating database '{Database}'");
                string createDb = $"CREATE DATABASE [{Database}]";
                QueryHandler.ExecuteNonQuery(createDb);
            }
        }


        private static void CheckDbForUpgradeTables()
        {
            string query =
                $"SELECT COUNT(*) FROM {Database}.sys.tables " +
                $"WHERE [name] IN ('{UpgradeTableName}', '{UpgradeErrorTableName}')";
            var result = QueryHandler.GetQueryResults(query);
            if((int)result.Rows[0].ItemArray[0] != 2)
            {
                ConsoleLogger.LogLine("Building missing database tables");
                query =
                    $"USE {Database} " +
                    // Build Upgrade Table
                    $"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = '{UpgradeTableName}') " +
                    "BEGIN " +
                    $"CREATE TABLE {UpgradeTableName} ( " +
                    $"{UpgradeTableName}Id INTEGER NOT NULL UNIQUE IDENTITY(1, 1), " +
                    "ScriptFile NVARCHAR(255) NOT NULL, " +
                    "ExecutedTime DATETIME NOT NULL DEFAULT GETUTCDATE(), " +
                    "IsSuccessful BIT NOT NULL DEFAULT 0, " +
                    $"PRIMARY KEY ({UpgradeTableName}Id) " +
                    ") END " +
                    // Build Upgrade Error Table
                    $"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = '{UpgradeErrorTableName}') " +
                    "BEGIN " +
                    $"CREATE TABLE {UpgradeErrorTableName} ( " +
                    $"{UpgradeErrorTableName}Id INTEGER NOT NULL UNIQUE IDENTITY(1, 1), " +
                    $"{UpgradeTableName}Id INTEGER NOT NULL, " +
                    "ErrorMessage NVARCHAR(MAX) NOT NULL DEFAULT GETUTCDATE(), " +
                    $"PRIMARY KEY ({UpgradeErrorTableName}Id), " +
                    $"FOREIGN KEY ({UpgradeTableName}Id) REFERENCES {UpgradeTableName}({UpgradeTableName}Id) " +
                    ") END ";
                QueryHandler.ExecuteNonQuery(query);
            }
        }


        private static void GetScriptsToRun()
        {
            UpgradeScriptDirectory = Path.GetFullPath(UpgradeScriptDirectory);
            if(!Directory.Exists(UpgradeScriptDirectory))
            {
                Directory.CreateDirectory(UpgradeScriptDirectory);                
                return;
            }
            string[] sqlFiles = Directory.GetFiles(UpgradeScriptDirectory, "*.sql")
                .OrderBy(x => decimal.Parse(Path.GetFileName(x).Split('_')[0]))
                .ToArray();
            string query =
                $"USE {Database} " +
                $"SELECT DISTINCT ScriptFile FROM {UpgradeTableName} WHERE IsSuccessful = 1";
            var queryResult = QueryHandler.GetQueryResults(query);
            List<string> successfulScripts = new(queryResult.Rows.Count);
            foreach(DataRow row in queryResult.Rows)
            {
                successfulScripts.Add(row["ScriptFile"].ToString().ToUpper());
            }
            List<string> scriptsToRun = new(sqlFiles.Length);
            string fileName = string.Empty;
            foreach(string file in sqlFiles)
            {
                fileName = Path.GetFileName(file).ToUpper();
                if(!successfulScripts.Contains(fileName))
                {
                    scriptsToRun.Add(file);
                }
            }
            SqlScripts = scriptsToRun;
            ParseGuidMonikers();
        }


        private static void RunUpgradeScripts()
        {
            if(SqlScripts.Count == 0)
            {
                ConsoleLogger.LogLine("No new scripts found");
                return;
            }
            string useDatabaseQuery = $"USE {Database}\r\nGO\r\n";
            Regex goSplit = new("\r\n[ \t]{0,}GO([ \t]{0,}\r\n{0,}){1,}");
            foreach(string file in SqlScripts)
            {
                ConsoleLogger.LogLine($"Running script '{file}'");
                string fileContent = useDatabaseQuery + File.ReadAllText(file);
                string[] statements = goSplit.Split(fileContent);
                try
                {
                    foreach(string statement in statements)
                    {
                        QueryHandler.ExecuteNonQuery(statement);
                    }
                }
                catch(Exception error)
                {
                    ConsoleLogger.LogError("ERROR", error);
                    LogUpgradeScriptRun(file, error);
                    continue;
                }
                LogUpgradeScriptRun(file, null);
            }
        }


        #region Helper Methods


        private static void LogUpgradeScriptRun(string filePath, Exception error)
        {
            int isSuccessful = (error == null ? 1 : 0);
            filePath = Path.GetFileName(filePath).Replace("'", "''");
            string query = $"INSERT INTO {UpgradeTableName} (ScriptFile, IsSuccessful) VALUES " + 
                $"('{filePath}', {isSuccessful})";
            QueryHandler.ExecuteNonQuery(query);
            if(error != null)
            {
                string message = error.Message.Replace("'", "''");
                query = $"INSERT INTO {UpgradeTableName} (UpgradeScriptRunId, ErrorMessage) " +
                    $"SELECT TOP 1 UpgradeScriptRunId, '{message}' FROM {UpgradeTableName} WHERE ScriptFile = '{filePath}' " +
                    "ORDER BY ExecutedTime DESC";
                QueryHandler.ExecuteNonQuery(query);
            }
        }

        private static void ParseGuidMonikers()
        {
            string newFileContent = string.Empty;
            foreach(string file in SqlScripts)
            {
                newFileContent = AssignGuids(file);
                File.WriteAllText(file, newFileContent);
            }
        }


        private static string AssignGuids(string filePath)
        {
            string fileContents = File.ReadAllText(filePath);
            string buffer = string.Empty;
            string result = string.Empty;
            Dictionary<string, string> savedPrimaryKeys = new(20);

            for(int i = 0; i < fileContents.Length; i++)
            {
                char c = fileContents[i];
                if(c == '{')
                {
                    buffer = c.ToString();
                    do
                    {
                        i++;
                        buffer += fileContents[i];
                    }
                    while (fileContents[i] != '}' && i < fileContents.Length);
                    if(buffer.StartsWith('{' + GuidMonikerPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(buffer.Equals($"{{{GuidMonikerPrefix}}}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            result += tnORMShared.NewGuid();
                        }
                        else
                        {
                            buffer = buffer.ToLower();
                            if(!savedPrimaryKeys.ContainsKey(buffer))
                            {
                                savedPrimaryKeys[buffer] = $"{tnORMShared.NewGuid()}";
                            }
                            result += savedPrimaryKeys[buffer];
                        }
                    }
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        #endregion
    }
}