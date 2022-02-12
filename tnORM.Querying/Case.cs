using tnORM.Shared;

namespace tnORM.Querying
{
    public class Case
    {
        private string? ElseValue { get; set; }

        private readonly string Alias = string.Empty;
        
        private readonly Dictionary<SqlClause, string> Cases = new();


        #region Constructors

        public Case() { }


        public Case(string alias)
        {
            Alias = alias;
        }

        #endregion


        public Case When(SqlPredicate predicate, object value)
        {
            var clause = new SqlClause(predicate);
            return When(clause, value);
        }


        public Case When(SqlClause clause, object value)
        {
            Cases[clause] = value.ToSqlString();
            return this;
        }


        public Case Else(object value)
        {
            ElseValue = value.ToSqlString();
            return this;
        }


        public override string ToString()
        {
            string result = "CASE ";
            foreach(var keyValuePair in Cases)
            {
                result += $"WHEN {keyValuePair.Key} THEN {keyValuePair.Value} ";
            }
            if(ElseValue != null)
            {
                result += $"ELSE {ElseValue} ";
            }
            result += "END";
            if (!string.IsNullOrEmpty(Alias))
            {
                result += $" AS [{Alias}]";
            }
            return result;
        }


        public static implicit operator string(Case c)
        {
            return c.ToString();
        }
    }
}
