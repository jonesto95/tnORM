using tnORM.Querying;

namespace tnORM.Tests.Runners
{
    internal class SqlFieldTestRunner : TestRunnerBase
    {
        /// <summary>
        /// Tests the creation of an empty SqlField.
        /// This is not allowed and should throw an EmptyFieldException.
        /// </summary>
        public void Test001()
        {
            try
            {
                SqlField field = new();
                throw new FailedTestCaseException();
            }
            catch (EmptyFieldException){ }
            catch(Exception e)
            {
                throw new FailedTestCaseException(e);
            }
        }


        /// <summary>
        /// Tests the creation of a SqlField using the standard constructor.
        /// </summary>
        public void Test002()
        {
            SqlField field = new("TestTable", "TestColumn");
            AssertEqual(field, "[TestTable].[TestColumn]");
        }


        /// <summary>
        /// Tests the creation of a SqlField using another SqlField as a basis,
        /// and adding a column alias.
        /// </summary>
        public void Test003()
        {
            SqlField field = new(BaseField, "bf");
            AssertEqual(field.ToStringWithAlias(), "[BaseTable].[BaseField] AS [bf]");
        }


        /// <summary>
        /// Tests the creation of a SqlField using another SqlField as a basis,
        /// and changing the table alias.
        /// </summary>
        public void Test004()
        {
            SqlField field = new("bt", BaseField);
            AssertEqual(field, "[bt].[BaseField]");
        }


        /// <summary>
        /// Tests the creation of a SqlField using another SqlField as a basis,
        /// changing the table alias and adding a colulmn alias.
        /// </summary>
        public void Test005()
        {
            SqlField field = new("bt", BaseField, "bf");
            AssertEqual(field.ToStringWithAlias(), "[bt].[BaseField] AS [bf]");
        }


        /// <summary>
        /// Tests the conversion of a field name to a readable field name.
        /// </summary>
        public void Test006()
        {
            string result = BaseField.GetReadableFieldName();
            AssertEqual(result, "Base Field");
        }


        /// <summary>
        /// Tests the conversion of a field name to a readable field name,
        /// using a reserved naming convention for boolean values.
        /// </summary>
        public void Test007()
        {
            SqlField field = new("TestTable", "IsActive", "a");
            string result = field.GetReadableFieldName();
            AssertEqual(result, "Is Active?");
        }
    }
}
