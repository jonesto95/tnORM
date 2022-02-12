using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class SqlUpdateTestRunner : TestRunnerBase
    {
        /// <summary>
        /// Creates a basic UPDATE statement with a single SET and WHERE predicate
        /// </summary>
        public void Test001()
        {
            var update = SqlUpdate
                .From<Customers>()
                .Set(CustomersFields.CustomersGuid, 55)
                .Where(CustomersFields.CustomersGuid.IsEqualTo(10));
            string result = update;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = "UPDATE [c] " +
                "SET [c].[CustomersGuid] = 55 " +
                $"FROM {databaseName}.[dbo].[Customers] AS [c] " +
                "WHERE ([c].[CustomersGuid] = 10)";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Attempts to create an UPDATE statement without any SETs.
        /// This is not allowed, and should throw an EmptyUpdateStatementException
        /// </summary>
        public void Test002()
        {
            try
            {
                var update = SqlUpdate.From<Customers>();
                string result = update;
                throw new FailedTestCaseException();
            }
            catch (EmptyUpdateStatementException) { }
            catch (Exception error)
            {
                throw new FailedTestCaseException(error);
            }
        }


        public void Test003()
        {
            var update = SqlUpdate
                .From<Customers>()
                    .InnerJoin<Employees>()
                        .On(EmployeesFields.EmployeesGuidField.IsEqualTo(CustomersFields.EmployeesGuid))
                .Set(CustomersFields.Name, EmployeesFields.Name);
            string result = update;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            string expected = "UPDATE [c] " +
                "SET [c].[Name] = [e].[Name] " +
                $"FROM {databaseName}.[dbo].[Customers] AS [c] " +
                $"INNER JOIN {databaseName}.[dbo].[Employees] AS [e] " +
                "ON ([e].[EmployeesGuid] = [c].[EmployeesGuid])";
            AssertEqual(result, expected);
        }
    }
}
