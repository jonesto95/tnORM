using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class SqlSelectTestRunner : TestRunnerBase
    {
        /// <summary>
        /// Tests a basic SELECT statement from a single table.
        /// </summary>
        public void Test001()
        {
            var sqlSelect = SqlSelect
                .From<Employees>();
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT * FROM {databaseName}.[dbo].[Employees] AS [e]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement from a single table with a custom alias
        /// </summary>
        public void Test002()
        {
            var sqlSelect = SqlSelect
                .From<Employees>("emp");
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT * FROM {databaseName}.[dbo].[Employees] AS [emp]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement from a single table with custom fields selected
        /// </summary>
        public void Test003()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .Select(1, "GG", CustomersFields.Name);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT 1, 'GG', [c].[Name] FROM {databaseName}.[dbo].[Customers] AS [c]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a SELECT statement containing both an INNER JOIN to a separate table, as well as a WHERE clause
        /// </summary>
        public void Test004()
        {
            var now = DateTime.Now.AddDays(-14);
            var sqlSelect = SqlSelect
                .From<Employees>()
                .Select("COUNT(*)")
                    .InnerJoin<Customers>()
                        .On(EmployeesFields.EmployeesGuidField.IsEqualTo(CustomersFields.EmployeesGuid))
                .Where(EmployeesFields.HiredDate.GreaterThan(now));
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT COUNT(*) FROM {databaseName}.[dbo].[Employees] AS [e] " +
                $"INNER JOIN {databaseName}.[dbo].[Customers] AS [c] ON ([e].[EmployeesGuid] = [c].[EmployeesGuid]) " +
                $"WHERE ([e].[HiredDate] > {now.ToSqlString()})";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a SELECT statement containing both a CROSS JOIN to a separate table, as well as a WHERE clause
        /// </summary>
        public void Test005()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .Select(CustomersFields.Name)
                .CrossJoin<Employees>()
                .Where(CustomersFields.EmployeesGuid.IsNotNull);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT [c].[Name] FROM {databaseName}.[dbo].[Customers] AS [c] " +
                $"CROSS JOIN {databaseName}.[dbo].[Employees] AS [e] " +
                "WHERE ([c].[EmployeesGuid] IS NOT NULL)";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement from a single table using a TOP WITH TIES directive.
        /// </summary>
        public void Test006()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .Top(10, true);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT TOP 10 WITH TIES * FROM {databaseName}.[dbo].[Customers] AS [c]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement from a single table using a TOP PERCENT WITH TIES directive.
        /// </summary>
        public void Test007()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .TopPercent(80, true)
                .OrderBy(CustomersFields.CustomersGuid, true);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT TOP 80 PERCENT WITH TIES * FROM {databaseName}.[dbo].[Customers] AS [c] " +
                "ORDER BY [c].[CustomersGuid] DESC";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement from a single table using a GROUP BY clause
        /// </summary>
        public void Test008()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .GroupBy(CustomersFields.Name);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT * FROM {databaseName}.[dbo].[Customers] AS [c] GROUP BY [c].[Name]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement containing both an INNER JOIN and a compound WHERE clause
        /// </summary>
        public void Test009()
        {
            var now = DateTime.Now.AddDays(-14);
            var sqlSelect = SqlSelect
                .From<Employees>()
                .Select("COUNT(*)")
                    .InnerJoin<Customers>()
                        .On(EmployeesFields.EmployeesGuidField.IsEqualTo(CustomersFields.EmployeesGuid))
                .Where(EmployeesFields.HiredDate.GreaterThan(now))
                    .And(CustomersFields.EmployeesGuid.IsNotNull);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT COUNT(*) FROM {databaseName}.[dbo].[Employees] AS [e] " +
                $"INNER JOIN {databaseName}.[dbo].[Customers] AS [c] ON ([e].[EmployeesGuid] = [c].[EmployeesGuid]) " +
                $"WHERE ([e].[HiredDate] > {now.ToSqlString()}) AND ([c].[EmployeesGuid] IS NOT NULL)";
            AssertEqual(result, expected);
        }




        /// <summary>
        /// Tests a basic SELECT statement from a single table using an OFFSET FETCH directive
        /// </summary>
        public void Test010()
        {
            var sqlSelect = SqlSelect
                .From<Customers>()
                .OrderBy(CustomersFields.CustomersGuid, true)
                .Offset(10).FetchNextOnly(5);
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT * FROM {databaseName}.[dbo].[Customers] AS [c] " +
                "ORDER BY [c].[CustomersGuid] DESC OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a basic SELECT statement containing both an INNER JOIN and a compound WHERE clause,
        /// using a NOLOCK directive
        /// </summary>
        public void Test011()
        {
            var now = DateTime.Now.AddDays(-14);
            var sqlSelect = SqlSelect
                .From<Employees>()
                .Nolock()
                .Select("COUNT(*)")
                    .InnerJoin<Customers>()
                        .On(EmployeesFields.EmployeesGuidField.IsEqualTo(CustomersFields.EmployeesGuid))
                .Where(EmployeesFields.HiredDate.GreaterThan(now));
            string result = sqlSelect;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"SELECT COUNT(*) FROM {databaseName}.[dbo].[Employees] AS [e] (NOLOCK) " +
                $"INNER JOIN {databaseName}.[dbo].[Customers] AS [c] (NOLOCK) ON ([e].[EmployeesGuid] = [c].[EmployeesGuid]) " +
                $"WHERE ([e].[HiredDate] > {now.ToSqlString()})";
            AssertEqual(result, expected);
        }
    }
}
