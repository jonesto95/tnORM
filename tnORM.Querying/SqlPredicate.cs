using tnORM.Shared;

namespace tnORM.Querying
{
    public struct SqlPredicate
    {
        private object Value1 { get; set; }
        private object Value2 { get; set; } = null;
        private Comparison Comparison { get; set; }

        #region Constructors and Construction Helpers

        /// <summary>
        /// Create predicates using IS NULL or IS NOT NULL directives
        /// </summary>
        public SqlPredicate(object value1, Comparison comparison)
        {
            Value1 = value1;
            Comparison = comparison;
            if(!Comparison.Equals(Comparison.IsNull) 
                && !Comparison.Equals(Comparison.IsNotNull))
            {
                throw new InvalidClauseException(this);
            }
        }


        /// <summary>
        /// Create predicates comparing two values
        /// </summary>
        public SqlPredicate(object value1, Comparison comparison, object value2)
        {
            Value1 = value1;
            Comparison = comparison;
            Value2 = value2.ToSqlString();
            if(Comparison.Equals(Comparison.IsNull)
                || Comparison.Equals(Comparison.IsNotNull)
                || Comparison.Equals(Comparison.Between)
                || Comparison.Equals(Comparison.NotBetween)
                || ComparisonIsExists(Comparison))
            {
                throw new InvalidClauseException(this);
            }
            if(Comparison.Equals(Comparison.In) || Comparison.Equals(Comparison.NotIn))
            {
                BuildInList(value2);
            }
        }


        /// <summary>
        /// Create predicates using BETWEEN or NOT BETWEEN directives
        /// </summary>
        public SqlPredicate(object value1, Comparison comparison, object value2, object value3)
        {
            Value1 = value1;
            Comparison = comparison;
            if(ComparisonIsIn(Comparison))
            {
                BuildInList(value2, value3);
            }
            else if (Comparison.Equals(Comparison.Between) || Comparison.Equals(Comparison.NotBetween))
            {
                Value2 = $"{value2.ToSqlString()} AND {value3.ToSqlString()}";
            }
            else
            {
                throw new InvalidClauseException(this);
            }
        }


        /// <summary>
        /// Create predicates using IN or NOT IN directives
        /// </summary>
        public SqlPredicate(object value1, Comparison comparison, params object[] values)
        {
            Value1 = value1;
            Comparison = comparison;
            if (!ComparisonIsIn(Comparison))
            {
                throw new InvalidClauseException(this);
            }
            BuildInList(values);
        }


        /// <summary>
        /// Create predicates using EXISTS or NOT EXISTS directives
        /// </summary>
        public SqlPredicate(Comparison comparison, SqlSelect select)
        {
            Value1 = null;
            Comparison = comparison;
            Value2 = $"({select})";
            if(!ComparisonIsExists(Comparison))
            {
                throw new InvalidClauseException(this);
            }
        }


        private void BuildInList(params object[] values)
        {
            string temp = "(";
            foreach(object value in values)
            {
                temp += $"{value.ToSqlString()}, ";
            }
            Value2 = temp[..^2] + ")";
        }


        private bool ComparisonIsIn(Comparison comparison)
        {
            return Comparison.Equals(Comparison.In) || Comparison.Equals(Comparison.NotIn);
        }


        private bool ComparisonIsExists(Comparison comparison)
        {
            return Comparison.Equals(Comparison.Exists) || Comparison.Equals(Comparison.NotExists);
        }

        #endregion

        public override string ToString()
        {
            string result = string.Empty;
            if(Value1 != null)
            {
                result = $"{Value1.ToSqlString()} ";
            }
            result += $"{Comparison}";
            if(Value2 != null)
            {
                result += $" {Value2}";
            }
            return result;
        }


        public static implicit operator string(SqlPredicate c)
        {
            return c.ToString();
        }
    }


    public class InvalidClauseException : Exception
    {
        public InvalidClauseException(string clause)
            : base($"Clause {clause} is not valid")
        { }
    }
}
