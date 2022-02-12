namespace tnORM.Querying
{
    public class SqlClause
    {
        private string ClauseString;

        #region Constructors

        public SqlClause()
        {
            ClauseString = string.Empty;
        }


        public SqlClause(SqlClause clause)
        {
            ClauseString = $"({clause.ClauseString})";
        }


        public SqlClause(SqlPredicate predicate)
        {
            ClauseString = predicate;
        }


        public SqlClause(object value1, Comparison comparison)
        {
            ClauseString = new SqlPredicate(value1, comparison);
        }


        public SqlClause(object value1, Comparison comparison, object value2)
        {
            ClauseString = new SqlPredicate(value1, comparison, value2);
        }


        public SqlClause(object value1, Comparison comparison, object value2, object value3)
        {
            ClauseString = new SqlPredicate(value1, comparison, value2, value3);
        }


        public SqlClause(object value1, Comparison comparison, params object[] values)
        {
            ClauseString = new SqlPredicate(value1, comparison, values);
        }


        public SqlClause(Comparison comparison, SqlSelect query)
        {
            ClauseString = new SqlPredicate(comparison, query);
        }

        #endregion


        #region AND conjunction methods

        public SqlClause And(SqlPredicate predicate)
        {
            return AddPredicate("AND", predicate);
        }


        public SqlClause And(SqlClause clause)
        {
            return AddClause("AND", clause);
        }


        public SqlClause And(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return AddPredicate("AND", predicate);
        }


        public SqlClause And(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return AddPredicate("AND", predicate);
        }


        public SqlClause And(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return AddPredicate("AND", predicate);
        }


        public SqlClause And(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return AddPredicate("AND", predicate);
        }


        public SqlClause And(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return AddPredicate("AND", predicate);
        }

        #endregion


        #region OR conjunction methods

        public SqlClause Or(SqlPredicate predicate)
        {
            return AddPredicate("OR", predicate);
        }


        public SqlClause Or(SqlClause clause)
        {
            return AddClause("OR", clause);
        }


        public SqlClause Or(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return AddPredicate("OR", predicate);
        }


        public SqlClause Or(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return AddPredicate("OR", predicate);
        }


        public SqlClause Or(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return AddPredicate("OR", predicate);
        }


        public SqlClause Or(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return AddPredicate("OR", predicate);
        }


        public SqlClause Or(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return AddPredicate("OR", predicate);
        }

        #endregion


        #region Private chaining methods

        private SqlClause AddPredicate(string conjunction, SqlPredicate predicate)
        {
            var clause = new SqlClause(predicate);
            return AddClause(conjunction, clause);
        }


        private SqlClause AddClause(string conjunction, SqlClause clause)
        {
            if(!string.IsNullOrEmpty(ClauseString))
            {
                ClauseString += $" {conjunction} ";
            }
            ClauseString += $"({clause})";
            return this;
        }

        #endregion


        public override string ToString()
        {
            if (string.IsNullOrEmpty(ClauseString))
            {
                throw new EmptyClauseException();
            }

            return ClauseString;
        }


        public static implicit operator string(SqlClause c)
        {
            return c.ToString();
        }
    }


    public class EmptyClauseException : Exception
    {
        public EmptyClauseException()
            : base("An empty clause is not allowed") { }
    }
}
