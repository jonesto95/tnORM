using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class SqlDeleteTestRunner : TestRunnerBase
    {
        /// <summary>
        /// Tests a basic DELETE statement, deleting all records from a single table
        /// </summary>
        public void Test001()
        {
            var delete = SqlDelete
                .From<Customers>();
            string result = delete;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"DELETE [c] FROM {databaseName}.[dbo].[Customers] AS [c]";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests a DELETE statement containing an INNER JOIN and a WHERE clause
        /// </summary>
        public void Test002()
        {
            var delete = SqlDelete
                .From<Customers>()
                    .InnerJoin<Employees>()
                        .On(EmployeesFields.EmployeesGuidField.IsEqualTo(CustomersFields.CustomersGuid))
                .Where(EmployeesFields.HiredDate.IsNull);
            string result = delete;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = $"DELETE [c] FROM {databaseName}.[dbo].[Customers] AS [c] " +
                $"INNER JOIN {databaseName}.[dbo].[Employees] AS [e] ON ([e].[EmployeesGuid] = [c].[CustomersGuid]) " +
                "WHERE ([e].[HiredDate] IS NULL)";
            AssertEqual(result, expected);
        }
    }
}
