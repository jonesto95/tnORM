using tnORM.Querying;
using tnORM.Querying.TableFields;
using tnORM.Shared;
using tnORM.Tests.Tables;

namespace tnORM.Tests.Runners
{
    internal class SqlPredicateTestRunner : TestRunnerBase
    {
        #region Predicates from Constructors

        /// <summary>
        /// Tests the construction of a SqlPredicate using the IS NULL check.
        /// </summary>
        public void Test001()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = new(value, Comparison.IsNull);
            AssertEqual(predicate, $"{value.ToSqlString()} IS NULL");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using the IS NOT NULL check.
        /// </summary>
        public void Test002()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = new(value, Comparison.IsNotNull);
            AssertEqual(predicate, $"{value.ToSqlString()} IS NOT NULL");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField as input.
        /// </summary>
        public void Test003()
        {
            SqlPredicate predicate = new(BaseField, Comparison.IsNotNull);
            AssertEqual(predicate, "[BaseTable].[BaseField] IS NOT NULL");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using an inappropriate Comparison object.
        /// This is not allowed and should throw an InvalidClauseException.
        /// </summary>
        public void Test004()
        {
            try
            {
                SqlPredicate predicate = new(BaseField, Comparison.IsEqualTo);
                throw new FailedTestCaseException();
            }
            catch (InvalidClauseException) { }
            catch(Exception e)
            {
                throw new FailedTestCaseException(e);
            }
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using an inappropriate Comparison object.
        /// This is not allowed and should throw an InvalidClauseException.
        /// </summary>
        public void Test005()
        {
            try
            {
                SqlPredicate predicate = new(BaseField, Comparison.Between);
                throw new FailedTestCaseException();
            }
            catch (InvalidClauseException) { }
            catch (Exception e)
            {
                throw new FailedTestCaseException(e);
            }
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and an Equals comparison.
        /// </summary>
        public void Test006()
        {
            SqlPredicate predicate = new(BaseField, Comparison.IsEqualTo, 10);
            AssertEqual(predicate, "[BaseTable].[BaseField] = 10");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Not Equals comparison.
        /// </summary>
        public void Test007()
        {
            SqlPredicate predicate = new(BaseField, Comparison.NotEqualTo, "G");
            AssertEqual(predicate, "[BaseTable].[BaseField] != 'G'");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using an inappropriate Comparison object.
        /// This is not allowed and should throw an InvalidClauseException.
        /// </summary>
        public void Test008()
        {
            try
            {
                SqlPredicate predicate = new(BaseField, Comparison.Between, 10);
                throw new FailedTestCaseException();
            }
            catch (InvalidClauseException) { }
            catch (Exception e)
            {
                throw new FailedTestCaseException(e);
            }
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Less Than comparison.
        /// </summary>
        public void Test009()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = new(BaseField, Comparison.LessThan, value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] < {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Greater Than Or Equal To comparison.
        /// </summary>
        public void Test010()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = new(BaseField, Comparison.GreaterThanOrEqualTo, value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] >= {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Between comparison.
        /// </summary>
        public void Test011()
        {
            int value1 = GetRandomIntValue();
            int value2 = GetRandomIntValue();
            SqlPredicate predicate = new(BaseField, Comparison.Between, value1, value2);
            AssertEqual(predicate, $"[BaseTable].[BaseField] BETWEEN {value1.ToSqlString()} AND {value2.ToSqlString()}");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Not Between comparison
        /// between two string values.
        /// </summary>
        public void Test012()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            SqlPredicate predicate = new(BaseField, Comparison.NotBetween, value1, value2);
            AssertEqual(predicate, $"[BaseTable].[BaseField] NOT BETWEEN {value1.ToSqlString()} AND {value2.ToSqlString()}");
        }



        /// <summary>
        /// Tests the construction of a SqlPredicate using an inappropriate Comparison object.
        /// This is not allowed and should throw an InvalidClauseException.
        /// </summary>
        public void Test013()
        {
            try
            {
                SqlPredicate predicate = new(BaseField, Comparison.IsNotNull, 6, 10);
                throw new FailedTestCaseException();
            }
            catch (InvalidClauseException) { }
            catch (Exception e)
            {
                throw new FailedTestCaseException(e);
            }
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a In comparison
        /// </summary>
        public void Test014()
        {
            int value1 = GetRandomIntValue();
            int value2 = GetRandomIntValue();
            SqlPredicate predicate = new(BaseField, Comparison.In, value1, value2);
            AssertEqual(predicate, $"[BaseTable].[BaseField] IN ({value1.ToSqlString()}, {value2.ToSqlString()})");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Not In comparison
        /// </summary>
        public void Test015()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            string value3 = GetRandomStringValue();
            string value4 = GetRandomStringValue();
            string value5 = GetRandomStringValue();
            SqlPredicate predicate = new(BaseField, Comparison.NotIn, value1, value2, value3, value4, value5);
            AssertEqual(predicate, $"[BaseTable].[BaseField] NOT IN ({value1.ToSqlString()}, {value2.ToSqlString()}, {value3.ToSqlString()}, {value4.ToSqlString()}, {value5.ToSqlString()})");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using an Exists comparison and a selection query.
        /// </summary>
        public void Test016()
        {
            SqlSelect query = SqlSelect.From<Customers>()
                .Select(CustomersFields.CustomersGuid)
                .Where(CustomersFields.Name.Like("%steve%"));
            SqlPredicate predicate = new(Comparison.Exists, query);
            string databaseName = tnORMConfig.GetDatabaseName("TestDb");
            AssertEqual(predicate, $"EXISTS (SELECT [c].[CustomersGuid] FROM {databaseName}.[dbo].[Customers] AS [c] WHERE ([c].[Name] LIKE '%steve%'))");
        }

        #endregion


        #region Predicate from SqlFields

        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Equal comparison.
        /// </summary>
        public void Test017()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.IsEqualTo(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] = {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Equal comparison.
        /// </summary>
        public void Test018()
        {
            SqlPredicate predicate = BaseField.IsEqualTo(true);
            AssertEqual(predicate, "[BaseTable].[BaseField] = 1");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Not Equal comparison.
        /// </summary>
        public void Test019()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.NotEqualTo(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] != {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Greater Than comparison.
        /// </summary>
        public void Test020()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.GreaterThan(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] > {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Greater Than Or Equal To comparison.
        /// </summary>
        public void Test021()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.GreaterThanOrEqualTo(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] >= {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Less Than comparison.
        /// </summary>
        public void Test022()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.LessThan(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] < {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Less Than Or Equal To comparison.
        /// </summary>
        public void Test023()
        {
            object value = GetRandomValue();
            SqlPredicate predicate = BaseField.LessThanOrEqualTo(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] <= {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Like comparison.
        /// </summary>
        public void Test024()
        {
            object value = GetRandomStringValue();
            SqlPredicate predicate = BaseField.Like(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] LIKE {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Not Like comparison.
        /// </summary>
        public void Test025()
        {
            object value = GetRandomStringValue();
            SqlPredicate predicate = BaseField.NotLike(value);
            AssertEqual(predicate, $"[BaseTable].[BaseField] NOT LIKE {value.ToSqlString()}");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the In comparison.
        /// </summary>
        public void Test026()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            string value3 = GetRandomStringValue();
            string value4 = GetRandomStringValue();
            string value5 = GetRandomStringValue();
            SqlPredicate predicate = BaseField.In(value1, value2, value3, value4, value5);
            AssertEqual(predicate, $"[BaseTable].[BaseField] IN ({value1.ToSqlString()}, {value2.ToSqlString()}, {value3.ToSqlString()}, {value4.ToSqlString()}, {value5.ToSqlString()})");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Not In comparison.
        /// </summary>
        public void Test027()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            string value3 = GetRandomStringValue();
            string value4 = GetRandomStringValue();
            string value5 = GetRandomStringValue();
            SqlPredicate predicate = BaseField.NotIn(value1, value2, value3, value4, value5);
            AssertEqual(predicate, $"[BaseTable].[BaseField] NOT IN ({value1.ToSqlString()}, {value2.ToSqlString()}, {value3.ToSqlString()}, {value4.ToSqlString()}, {value5.ToSqlString()})");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Is Null comparison.
        /// </summary>
        public void Test028()
        {
            SqlPredicate predicate = BaseField.IsNull;
            AssertEqual(predicate, $"[BaseTable].[BaseField] IS NULL");
        }


        /// <summary>
        /// Tests the creation of a SqlPredicate from a SqlField method, using the Is Not Null comparison.
        /// </summary>
        public void Test029()
        {
            SqlPredicate predicate = BaseField.IsNotNull;
            AssertEqual(predicate, $"[BaseTable].[BaseField] IS NOT NULL");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Between comparison.
        /// </summary>
        public void Test030()
        {
            int value1 = GetRandomIntValue();
            int value2 = GetRandomIntValue();
            SqlPredicate predicate = BaseField.Between(value1, value2);
            AssertEqual(predicate, $"[BaseTable].[BaseField] BETWEEN {value1.ToSqlString()} AND {value2.ToSqlString()}");
        }


        /// <summary>
        /// Tests the construction of a SqlPredicate using a SqlField and a Not Between comparison
        /// between two string values.
        /// </summary>
        public void Test031()
        {
            string value1 = GetRandomStringValue();
            string value2 = GetRandomStringValue();
            SqlPredicate predicate = BaseField.NotBetween(value1, value2);
            AssertEqual(predicate, $"[BaseTable].[BaseField] NOT BETWEEN {value1.ToSqlString()} AND {value2.ToSqlString()}");
        }

        #endregion
    }
}
