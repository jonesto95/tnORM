using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class SqlClauseTestRunner : TestRunnerBase
    {
        /// <summary>
        /// Tests the ToString function call against an empty SqlClause.
        /// This is not allowed and should throw an EmptyClauseException.
        /// </summary>
        public void Test001()
        {
            try
            {
                SqlClause clause = new();
                string result = clause.ToString();
                throw new FailedTestCaseException();
            }
            catch(EmptyClauseException) { }
            catch(Exception error)
            {
                throw new FailedTestCaseException(error);
            }
        }


        /// <summary>
        /// Tests the creation of a SqlClause through a SqlPredicate object.
        /// </summary>
        public void Test002()
        {
            var now = DateTime.Now;
            SqlClause predicate = new(CustomersFields.LastModifiedTime, Comparison.LessThan, now);
            string result = predicate;
            AssertEqual(result, $"[c].[LastModifiedTime] < {now.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlClause
        /// </summary>
        public void Test003()
        {
            SqlClause clause = new(CustomersFields.Name, Comparison.IsNotNull);
            string result = clause;
            AssertEqual(result, $"[c].[Name] IS NOT NULL");
        }


        /// <summary>
        /// Tests the creation of a SqlClause
        /// </summary>
        public void Test004()
        {
            int value = GetRandomIntValue();
            SqlClause clause = new(CustomersFields.CustomersGuid, Comparison.GreaterThan, value);
            string result = clause;
            AssertEqual(result, $"[c].[CustomersGuid] > {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlClause
        /// </summary>
        public void Test005()
        {
            var startTime = GetRandomDateTimeValue();
            var endTime = startTime.AddSeconds(GetRandomIntValue());
            SqlClause clause = new(CustomersFields.CreatedDate, Comparison.Between, startTime, endTime);
            string result = clause;
            AssertEqual(result, $"[c].[CreatedDate] BETWEEN {startTime.ToSqlString()} AND {endTime.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlClause
        /// </summary>
        public void Test006()
        {
            int value1 = GetRandomIntValue();
            int value2 = GetRandomIntValue();
            int value3 = GetRandomIntValue();
            SqlClause clause = new(CustomersFields.CreatedDate, Comparison.In, value1, value2, value3);
            string result = clause;
            AssertEqual(result, $"[c].[CreatedDate] IN ({value1.ToSqlString()}, {value2.ToSqlString()}, {value3.ToSqlString()})");
        }


        /// <summary>
        /// Tests the creation of a SqlClause
        /// </summary>
        public void Test007()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            SqlSelect query = SqlSelect
                .From<Customers>()
                .Where(CustomersFields.Name.Between(value1, value2));
            SqlClause clause = new(Comparison.Exists, query);
            string result = clause;
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            AssertEqual(result, $"EXISTS (SELECT * FROM {databaseName}.[dbo].[Customers] AS [c] WHERE ([c].[Name] BETWEEN {value1.ToSqlString()} AND {value2.ToSqlString()}))");
        }


        /// <summary>
        /// Tests the creation of a SqlClause, with an appended AND predicate
        /// </summary>
        public void Test008()
        {
            int value = GetRandomIntValue();
            string strValue1 = GetRandomStringValue();
            string strValue2 = GetRandomStringValue();
            string strValue3 = GetRandomStringValue();
            var predicate1 = CustomersFields.CustomersGuid.IsEqualTo(value);
            var predicate2 = CustomersFields.Name.NotIn(strValue1, strValue2, strValue3);
            var clause = new SqlClause(predicate1).And(predicate2);
            string result = clause;
            AssertEqual(result, $"[c].[CustomersGuid] = {value.ToSqlString()} AND ([c].[Name] NOT IN ({strValue1.ToSqlString()}, {strValue2.ToSqlString()}, {strValue3.ToSqlString()}))");
        }


        /// <summary>
        /// Tests the creation of a SqlClause by chaining other SqlClauses together
        /// </summary>
        public void Test009()
        {
            string value1 = GetRandomStringValue();
            int intValue = GetRandomIntValue() / 100;
            DateTime date = DateTime.Now.AddDays(-intValue);
            var clause1 = new SqlClause(EmployeesFields.Name.Like(value1))
                .Or(CustomersFields.Name.Like(value1));
            var clause2 = new SqlClause(EmployeesFields.CreatedDate.GreaterThan(date))
                .Or(CustomersFields.CreatedDate.GreaterThan(date));
            var clause = new SqlClause(clause1).And(clause2);
            string result = clause;
            string expected = $"([e].[Name] LIKE {value1.ToSqlString()} OR ([c].[Name] LIKE {value1.ToSqlString()})) " +
                $"AND ([e].[CreatedDate] > {date.ToSqlString()} OR ([c].[CreatedDate] > {date.ToSqlString()}))";
            AssertEqual(result, expected);
        }


        /// <summary>
        /// Tests the creation of a SqlClause by chaining other SqlClauses together
        /// </summary>
        public void Test010()
        {
            string value1 = GetRandomStringValue();
            int intValue = GetRandomIntValue() / 100;
            DateTime date = DateTime.Now.AddDays(-intValue);
            var clause = new SqlClause(EmployeesFields.Name.Like(value1))
                .Or(EmployeesFields.HiredDate.LessThanOrEqualTo(date))
                .And(CustomersFields.EmployeesGuid.IsNotNull);
            string result = clause;
            string expected = $"[e].[Name] LIKE {value1.ToSqlString()} OR ([e].[HiredDate] <= {date.ToSqlString()}) AND ([c].[EmployeesGuid] IS NOT NULL)";
            AssertEqual(result, expected);
        }
    }
}
