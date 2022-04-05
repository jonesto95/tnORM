using System.Data;
using tnORM.Shared;

namespace tnORM
{
    public static class ORMCodeGenerator
    {
        private static string UpgradeTableName { get; set; } = tnORMShared.UpgradeTableName;
        private static string CurrentDatabase { get; set; }
        private static string CurrentSchema { get; set; }
        private static string CurrentTableName { get; set; }
        private static string CurrentTableAlias { get; set; }
        private static string CurrentColumnName { get; set; }
        private static string OutputDirectory { get; set; }
        private static string NamespaceName { get; set; }
        private static int FieldPrecision { get; set; }
        private static byte FieldScale { get; set; }
        private static short? FieldMaxLength { get; set; }
        private static DataTable DatabaseSchema { get; set; }
        private static List<DataRow> CurrentTableSchema { get; set; } = new();


        public static void Run(string database, string namespaceName, string schemas)
        {
            CurrentDatabase = database;
            NamespaceName = namespaceName;
            string[] schemaList = schemas.Split(',');
            foreach(string schema in schemaList)
            {
                CurrentSchema = schema;
                PrepareOutputDirectory();
                GetDatabaseSchema();
                BuildClasses();
                BuildFunctionClass();
                BuildStoredProcedureClass();
            }
        }


        // Step 1
        private static void PrepareOutputDirectory()
        {
            OutputDirectory = tnORMConfig.GetString("CodeOutputDirectory");
            OutputDirectory += $"/{CurrentDatabase}.{CurrentSchema}";
            Directory.CreateDirectory(OutputDirectory);

            foreach(string file in Directory.GetFiles(OutputDirectory, "*.cs"))
            {
                File.Delete(file);
            }
        }


        // Step 2
        private static void GetDatabaseSchema()
        {
            string schemaQueryFilePath = tnORMConfig.GetString("SchemaQueryFilePath");
            if (!File.Exists(schemaQueryFilePath))
            {
                throw new FileNotFoundException("Schema query file not found", schemaQueryFilePath);
            }
            string schemaQuery = File.ReadAllText(schemaQueryFilePath);
            schemaQuery = schemaQuery.Replace("%_DATABASE_%", CurrentDatabase)
                .Replace("%_SCHEMA_%", CurrentSchema);
            DatabaseSchema = QueryHandler.GetQueryResults(schemaQuery);
        }


        // Step 3
        private static void BuildClasses()
        {
            int rowCursorIndex = 0;

            DataRow currentRow;
            while(rowCursorIndex < DatabaseSchema.Rows.Count)
            {
                currentRow = DatabaseSchema.Rows[rowCursorIndex];
                CurrentTableName = currentRow["TableName"].ToString();
                CurrentTableAlias = currentRow["TableAlias"].ToString();
                CurrentTableSchema.Add(currentRow);
                rowCursorIndex++;
                currentRow = DatabaseSchema.Rows[rowCursorIndex];
                while(rowCursorIndex < DatabaseSchema.Rows.Count
                    && CurrentTableName.Equals(currentRow["TableName"].ToString()))
                { 
                    CurrentTableSchema.Add(currentRow);
                    rowCursorIndex++;
                    if(rowCursorIndex < DatabaseSchema.Rows.Count)
                    {
                        currentRow = DatabaseSchema.Rows[rowCursorIndex];
                    }
                }
                BuildTableClasses();
                CurrentTableSchema.Clear();
            }
        }


        // Step 4
        private static void BuildTableClasses()
        {
            if (CurrentTableName.Equals(tnORMShared.UpgradeTableName, StringComparison.InvariantCultureIgnoreCase)
                || CurrentTableName.Equals(tnORMShared.UpgradeErrorTableName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            ConsoleLogger.LogLine($"Generating class file for table {CurrentDatabase}.{CurrentSchema}.{CurrentTableName}");
            string modelClass =
                "using tnORM.Shared;\r\n" +
                "using tnORM.Querying;\r\n" +
                "\r\n" +
                $"namespace tnORM.Model.{NamespaceName}.{CurrentSchema}.Tables\r\n" +
                "{\r\n" +
                $"\tpublic partial class {CurrentTableName} : tnORMTableBase\r\n" +
                "\t{\r\n" +
                "\t\tpublic override string DatabaseName\r\n" +
                "\t\t{\r\n" +
                "\t\t\tget\r\n" +
                "\t\t\t{\r\n" +
                "\t\t\t\tif(!string.IsNullOrEmpty(databaseName)) { return databaseName; }\r\n" +
                $"\t\t\t\tstring database = tnORMConfig.TryGetString(\"DatabaseOverrides:{NamespaceName}\");\r\n" +
                $"\t\t\t\tdatabaseName = database ?? \"{NamespaceName}\";\r\n" +
                "\t\t\t\treturn databaseName;\r\n" +
                "\t\t\t}\r\n" +
                "\t\t}\r\n" +
                "\t\tprivate string databaseName;\r\n" +
                "\r\n" +
                $"\t\tpublic override string SchemaName => \"{CurrentSchema}\";\r\n" +
                "\r\n" +
                $"\t\tpublic override string TableName => \"{CurrentTableName}\";\r\n" +
                "\r\n" +
                $"\t\tpublic override string TableAlias => \"{CurrentTableAlias}\";\r\n" +
                "\r\n" +
                $"\t\tpublic override {CurrentTableName}Fields Fields => new();\r\n" +
                "\r\n" +
                $"\t\tpublic new {CurrentTableName}Data Data {{ get; private set; }} = new();\r\n" +
                "\r\n" +
                "\t\tpublic override void SetDataField(string field, object value)\r\n" +
                "\t\t{\r\n" +
                "\t\t\tData = Data.SetProperty(field, value);\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n";

            string tableFieldClass = 
                $"\tpublic partial class {CurrentTableName}Fields : tnORMTableFieldCollection\r\n" +
                "\t{\r\n";

            string tableDataClass =
                $"\tpublic partial class {CurrentTableName}Data : tnORMTableDataCollection\r\n" +
                "\t{\r\n";

            string dataConstructor =
                $"\t\tpublic {CurrentTableName}Data()\r\n" +
                "\t\t{\r\n";

            string defaultStatement;
            int? dataType;
            bool isNullable, isPrimaryKey, isIdentity;
            foreach(var column in CurrentTableSchema)
            {
                CurrentColumnName = (string)column["ColumnName"];
                FieldMaxLength = (short?)column["MaxLength"];
                FieldPrecision = (int)column["Precision"];
                FieldScale = (byte)column["Scale"];
                isNullable = (bool)column["Nullable"];
                isPrimaryKey = (int)column["IsPrimaryKey"] == 1;
                isIdentity = (int)column["IsIdentity"] == 1;
                defaultStatement = column["Default"] == DBNull.Value ? null : column["Default"].ToString();
                dataType = (int?)column["DataType"];

                // Build Table Fields class
                if (isPrimaryKey)
                {
                    tableFieldClass += "\t\t[PrimaryKey]\r\n";
                }
                if (isIdentity)
                {
                    tableFieldClass += "\t\t[Identity]\r\n";
                }
                tableFieldClass += "\t\t[DatabaseColumn]\r\n" +
                    $"\t\tpublic static SqlField {CurrentColumnName} {{ get; }} = new(\"{CurrentTableAlias}\", \"{CurrentColumnName}\", \"{CurrentColumnName}\");\r\n" +
                    "\r\n";

                // Build Table Data class
                if (!string.IsNullOrEmpty(defaultStatement))
                {
                    dataConstructor += GetDefaultStatement(dataType.Value, defaultStatement);
                }
                string cSharpDataType = GetCSharpDataType(dataType.Value, isNullable);
                tableDataClass += $"\t\tpublic {cSharpDataType} {CurrentColumnName} {{ get; set; }}\r\n\r\n";
            }
            tableFieldClass += "\t}";
            dataConstructor += "\t\t}\r\n\t}\r\n}";

            string fileText = $"{modelClass}\r\n{tableFieldClass}\r\n{tableDataClass}\r\n{dataConstructor}";
            string outputPath = $"{OutputDirectory}/{CurrentTableName}.cs";
            File.WriteAllText(outputPath, fileText);
        }


        // Step 5
        private static void BuildFunctionClass()
        {
            string query =
                $"USE {CurrentDatabase}" +
                " SELECT o.[Name] AS [FunctionName], p.[Name] AS [ParamName]," +
                " p.user_type_id [DataType], p.is_nullable [Nullable]" +
                " FROM sys.parameters p" +
                " INNER JOIN sys.objects o ON o.object_id = p.object_id" +
                " INNER JOIN sys.schemas s ON s.schema_id = o.schema_id" +
                " WHERE [Type] IN ('FN', 'IF', 'AF', 'FS', 'FT')" +
                $" AND s.name = '{CurrentSchema}'";
            var functions = QueryHandler.GetQueryResults(query);
            if(functions.Rows.Count == 0)
            {
                return;
            }

            string functionClass =
                "using System.Data;\r\n" +
                "using tnORM.Shared;\r\n" +
                "using tnORM.Querying;\r\n" +
                "\r\n" +
                $"namespace tnORM.Model.{NamespaceName}.{CurrentSchema}.Functions\r\n" +
                "{\r\n" +
                "\tpublic partial class Functions\r\n" +
                "\t{\r\n" +
                "\t\tpublic static string DatabaseName\r\n" +
                "\t\t{\r\n" +
                "\t\t\tget\r\n" +
                "\t\t\t{\r\n" +
                "\t\t\t\tif(!string.IsNullOrEmpty(databaseName)) { return databaseName; }\r\n" +
                $"\t\t\t\tstring database = tnORMConfig.TryGetString(\"DatabaseOverrides:{NamespaceName}\");\r\n" +
                $"\t\t\t\tdatabaseName = database ?? \"{NamespaceName}\";\r\n" +
                "\t\t\t\treturn databaseName;\r\n" +
                "\t\t\t}\r\n" +
                "\t\t}\r\n" +
                "\t\tprivate static string databaseName;\r\n" +
                "\r\n";
            int i = 0;
            List<DataRow> parameters = new();
            DataRow currentRow;
            string currentFunction;
            while(i < functions.Rows.Count)
            {
                currentRow = functions.Rows[i];
                currentFunction = currentRow["FunctionName"].ToString();
                parameters.Add(currentRow);
                i++;
                if(i < functions.Rows.Count)
                {
                    currentRow = functions.Rows[i];
                    while(i < functions.Rows.Count
                        && currentFunction.Equals(currentRow["FunctionName"].ToString()))
                    {
                        parameters.Add(currentRow);
                        i++;
                        if(i < functions.Rows.Count)
                        {
                            currentRow = functions.Rows[i];
                        }
                    }
                }
                functionClass += GetFunctionMethodDefinition(parameters);
                parameters.Clear();
            }
            functionClass +=
                "\t}\r\n" +
                "}\r\n";
            string outputPath = $"{OutputDirectory}/Functions.cs";
            File.WriteAllText(outputPath, functionClass);
        }


        // Step 6
        private static void BuildStoredProcedureClass()
        {
            string query =
                $"USE {CurrentDatabase}" +
                " SELECT c.name [ProcName], ISNULL(p.name, '') [ParamName], p.user_type_id [DataType]," +
                " p.is_nullable [Nullable]" +
                " FROM sys.procedures c" +
                " LEFT JOIN sys.parameters p ON c.object_id = p.object_id" +
                " INNER JOIN sys.objects o ON o.object_id = c.object_id" +
                " INNER JOIN sys.schemas s ON s.schema_id = o.schema_id" +
                $" WHERE s.name = '{CurrentSchema}'" +
                " ORDER BY c.name, ISNULL(p.parameter_id, 1)";
            var storedProcedures = QueryHandler.GetQueryResults(query);
            if(storedProcedures.Rows.Count == 0)
            {
                return;
            }

            string storedProcedureClass =
                "using System.Data;\r\n" +
                "using tnORM.Shared;\r\n" +
                "using tnORM.Querying;\r\n" +
                "\r\n" +
                $"namespace tnORM.Model.{NamespaceName}.{CurrentSchema}.StoredProcedures\r\n" +
                "{\r\n" +
                "\tpublic partial class StoredProcedures\r\n" +
                "\t{\r\n";
            int i = 0;
            List<DataRow> parameters = new();
            DataRow currentRow;
            string currentProcedure;
            while(i < storedProcedures.Rows.Count)
            {
                currentRow = storedProcedures.Rows[i];
                currentProcedure = currentRow["ProcName"].ToString();
                parameters.Add(currentRow);
                i++;
                if(i < storedProcedures.Rows.Count)
                {
                    currentRow = storedProcedures.Rows[i];
                    while(i < storedProcedures.Rows.Count
                        && currentProcedure.Equals(currentRow["ProcName"].ToString()))
                    {
                        parameters.Add(currentRow);
                        i++;
                        if(i < storedProcedures.Rows.Count)
                        {
                            currentRow = storedProcedures.Rows[i];
                        }
                    }
                }
                storedProcedureClass += GetStoredProcedureMethodDefinition(parameters);
                parameters.Clear();
            }
            storedProcedureClass +=
                "\t}\r\n" +
                "}\r\n";
            string outputPath = $"{OutputDirectory}/StoredProcedures.cs";
            File.WriteAllText(outputPath, storedProcedureClass);
        }


        #region Helper Methods
        private static string GetFunctionMethodDefinition(List<DataRow> parameters)
        {
            string functionName = parameters[0]["FunctionName"].ToString();
            string cSharpParameterList = string.Empty;
            string sqlParameterList = string.Empty;
            foreach(var param in parameters)
            {
                string parameterName = param["ParamName"].ToString();
                if(parameterName.Length == 0)
                {
                    continue;
                }
                parameterName = parameterName[1..];
                string dataType = GetCSharpDataType((int)param["DataType"], (bool)param["Nullable"]);
                cSharpParameterList += $" {dataType} {parameterName},";
                sqlParameterList += $" {{{parameterName}?.ToSqlString() ?? \"NULL\"}},";
            }
            if(cSharpParameterList.Length > 0)
            {
                cSharpParameterList = cSharpParameterList[1..^1];
            }
            if(sqlParameterList.Length > 0)
            {
                sqlParameterList = sqlParameterList[..^1];
            }
            string result =
                $"\t\tpublic static DataTable {functionName}({cSharpParameterList})\r\n" +
                "\t\t{\r\n" +
                "\t\t\tDataTable result;\r\n" +
                $"\t\t\tstring sqlText = $\"USE {{DatabaseName}} SELECT [{CurrentSchema}].[{functionName}]({sqlParameterList})\";\r\n" +
                "\t\t\tresult = tnORMQueryInterface.ExecuteQueryText(sqlText);\r\n" +
                "\t\t\treturn result;\r\n" +
                "\t\t}\r\n" +
                "\r\n";
            return result;
        }


        private static string GetStoredProcedureMethodDefinition(List<DataRow> parameters)
        {
            string procedureName = parameters[0].ItemArray[0].ToString();
            string cSharpParameterList = string.Empty;
            string procedureCallParameterList = string.Empty;
            string sqlParameterList = string.Empty;
            string addParametersToCall = string.Empty;
            foreach(var param in parameters)
            {
                string parameterName = param["ParamName"].ToString();
                if (parameterName.Length == 0)
                {
                    continue;
                }
                parameterName = parameterName[1..];
                string dataType = GetCSharpDataType((int)param["DataType"], (bool)param["Nullable"]);
                cSharpParameterList += $" {dataType} {parameterName},";
                procedureCallParameterList += $" {parameterName},";
                addParametersToCall += $"\t\t\tcall[\"{parameterName}\"] = {parameterName};\r\n";
                sqlParameterList += $" {{{parameterName}?.ToSqlString() ?? \"NULL\"}},";
            }
            if(cSharpParameterList.Length > 0)
            {
                cSharpParameterList = cSharpParameterList[1..^1];
            }
            if (procedureCallParameterList.Length > 0)
            {
                procedureCallParameterList = procedureCallParameterList[1..^1];
            }
            if (sqlParameterList.Length > 0)
            {
                sqlParameterList = sqlParameterList[..^1];
            }
            string result =
                $"\t\tpublic static DataSet {procedureName}({cSharpParameterList})\r\n" +
                "\t\t{\r\n" +
                "\t\t\tDataSet result;\r\n" +
                $"\t\t\tstring sqlText = $\"EXEC {procedureName}{sqlParameterList}\";\r\n" +
                "\t\t\tresult = tnORMQueryInterface.ExecuteQueryTextIntoDataSet(sqlText);\r\n" +
                "\t\t\treturn result;\r\n" +
                "\t\t}\r\n" +
                "\r\n" +
                $"\t\tpublic static SqlStoredProcedureCall Create{procedureName}Call({cSharpParameterList})\r\n" +
                "\t\t{\r\n" +
                $"\t\t\tSqlStoredProcedureCall call = new(\"{procedureName}\");\r\n" +
                addParametersToCall +
                "\t\t\treturn call;\r\n" +
                "\t\t}\r\n" +
                "\r\n";
            return result;
        }


        private static string GetDefaultStatement(int dataType, string defaultStatement)
        {
            switch (dataType)
            {
                case 56: // INT
                    if(int.TryParse(defaultStatement[2..^2], out int intValue))
                    {
                        return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", {intValue});\r\n";
                    }
                    break;

                case 104: // BIT
                    char defaultValue = defaultStatement[2];
                    if ("01".Contains(defaultValue))
                    {
                        string val = (defaultValue == '1' ? "true" : "false");
                        return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", {val});\r\n";
                    }
                    break;

                case 108: // NUMERIC
                    if(defaultStatement.IndexOf('.') == -1)
                    {
                        defaultStatement = defaultStatement[..^2] + ".0))";
                    }
                    if (decimal.TryParse(defaultStatement[2..^2], out decimal decimalValue))
                    {
                        return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", {decimalValue}m);\r\n";
                    }
                    break;

                case 35:
                case 99:
                case 167:
                case 231: // string
                    if(defaultStatement.StartsWith("('") && defaultStatement.EndsWith("')"))
                    {
                        defaultStatement = defaultStatement[2..^2]
                            .Replace("''", "'");
                        return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", \"{defaultStatement}\");\r\n";
                    }
                    break;
            }

            string defaultStatementOverride = tnORMConfig.TryGetString($"DefaultValueMappings:{defaultStatement}");
            if (string.IsNullOrEmpty(defaultStatementOverride))
            {
                return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", tnORMQueryInterface.ExecuteQueryText(\"SELECT {defaultStatement}\").Get<object>(0));\r\n";
            }
            return $"\t\t\tthis.SetProperty(\"{CurrentColumnName}\", {defaultStatementOverride});\r\n";
        }


        private static string GetCSharpDataType(int userTypeId, bool isNullable)
        {
            string result = "";
            switch (userTypeId)
            {
                case 35:
                case 99:
                case 167:
                case 231:
                    result = "string";
                    break;

                case 36:
                    result = "Guid";
                    break;

                case 56:
                    result = "int";
                    break;

                case 61:
                    result = "DateTime";
                    break;

                case 104:
                    result = "bool";
                    break;

                case 108:
                    result = "decimal";
                    break;
            }
            if (isNullable)
            {
                result += "?";
            }
            return result;
        }

        #endregion
    }
}
