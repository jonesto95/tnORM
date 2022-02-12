using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class CaseTestRunner : TestRunnerBase
    {
        public void Test001()
        {
            var sqlCase = new Case()
                .When(CustomersFields.Name.IsNotNull, 5)
                .When(CustomersFields.Name.Like("%A%"), 10)
                .Else(20);
            string result = sqlCase;
            string expected = "CASE WHEN [c].[Name] IS NOT NULL THEN 5 WHEN [c].[Name] LIKE '%A%' THEN 10 ELSE 20 END";
            AssertEqual(result, expected);
        }


        public void Test002()
        {
            var sqlCase = new Case("Test")
                .When(CustomersFields.Name.IsNotNull, 5)
                .When(CustomersFields.Name.Like("%A%"), 10)
                .Else(20);
            string result = sqlCase;
            string expected = "CASE WHEN [c].[Name] IS NOT NULL THEN 5 WHEN [c].[Name] LIKE '%A%' THEN 10 ELSE 20 END AS [Test]";
            AssertEqual(result, expected);
        }
    }
}
