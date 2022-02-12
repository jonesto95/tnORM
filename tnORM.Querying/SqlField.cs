namespace tnORM.Querying
{
    public struct SqlField
    {
        public readonly string ColumnName { get; }

        public readonly string TableAlias { get; }

        public readonly string ColumnAlias { get; }


        #region Constructors

        /// <summary>
        /// Use of this constructor is not allowed
        /// </summary>
        /// <exception cref="EmptyFieldException">This exception will occur</exception>
        public SqlField()
        {
            throw new EmptyFieldException();
        }


        public SqlField(string tableAlias, string columnName)
        {
            TableAlias = tableAlias;
            ColumnName = columnName;
            ColumnAlias = null;
        }


        public SqlField(string tableAlias, string columnName, string columnAlias)
        {
            TableAlias = tableAlias;
            ColumnName = columnName;
            ColumnAlias = columnAlias;
        }


        public SqlField(SqlField baseField, string columnAlias)
        {
            TableAlias = baseField.TableAlias;
            ColumnName = baseField.ColumnName;
            ColumnAlias = columnAlias;
        }


        public SqlField(string tableAlias, SqlField baseField)
        {
            TableAlias = tableAlias;
            ColumnName = baseField.ColumnName;
            ColumnAlias = baseField.ColumnAlias;
        }


        public SqlField(string tableAlias, SqlField baseField, string columnAlias)
        {
            TableAlias = tableAlias;
            ColumnName = baseField.ColumnName;
            ColumnAlias = columnAlias;
        }

        #endregion


        #region Predicate methods

        public SqlPredicate IsEqualTo(object value)
        {
            return new(this, Comparison.IsEqualTo, value);
        }


        public SqlPredicate NotEqualTo(object value)
        {
            return new(this, Comparison.NotEqualTo, value);
        }


        public SqlPredicate LessThan(object value)
        {
            return new(this, Comparison.LessThan, value);
        }


        public SqlPredicate LessThanOrEqualTo(object value)
        {
            return new(this, Comparison.LessThanOrEqualTo, value);
        }


        public SqlPredicate GreaterThan(object value)
        {
            return new(this, Comparison.GreaterThan, value);
        }


        public SqlPredicate GreaterThanOrEqualTo(object value)
        {
            return new(this, Comparison.GreaterThanOrEqualTo, value);
        }


        public SqlPredicate Like(object value)
        {
            return new(this, Comparison.Like, value);
        }


        public SqlPredicate NotLike(object value)
        {
            return new(this, Comparison.NotLike, value);
        }


        public SqlPredicate In(params object[] values)
        {
            return new(this, Comparison.In, values);
        }


        public SqlPredicate NotIn(params object[] values)
        {
            return new(this, Comparison.NotIn, values);
        }


        public SqlPredicate IsNull
        {
            get
            {
                return new(this, Comparison.IsNull);
            }
        }


        public SqlPredicate IsNotNull
        {
            get
            {
                return new(this, Comparison.IsNotNull);
            }
        }


        public SqlPredicate Between(object value1, object value2)
        {
            return new(this, Comparison.Between, value1, value2);
        }


        public SqlPredicate NotBetween(object value1, object value2)
        {
            return new(this, Comparison.NotBetween, value1, value2);
        }

        #endregion


        public string GetReadableFieldName()
        {
            string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string fieldName = ColumnName;
            bool previousCharIsLowercase = false;
            string result = string.Empty;
            for(int i = 0; i < fieldName.Length; i++)
            {
                if (uppercaseLetters.Contains(fieldName[i]))
                {
                    if(previousCharIsLowercase)
                    {
                        result += " ";
                    }
                    previousCharIsLowercase = false;
                }
                else
                {
                    previousCharIsLowercase = true;
                }
                result += fieldName[i];
            }
            if(result.StartsWith("Is "))
            {
                result += "?";
            }
            return result;
        }


        public override string ToString()
        {
            return $"[{TableAlias}].[{ColumnName}]";
        }


        public string ToStringWithAlias()
        {
            string result = ToString();
            if (!string.IsNullOrEmpty(ColumnAlias))
            {
                result += $" AS [{ColumnAlias}]";
            }
            return result;
        }


        public static implicit operator string(SqlField s)
        {
            return s.ToString();
        }
    }


    public class EmptyFieldException : Exception
    {
        public EmptyFieldException()
            : base("An empty field cannot be created") { }
    }
}
