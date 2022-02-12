namespace tnORM.Querying
{
    public struct Comparison
    {
        public static Comparison IsEqualTo = new("=");
        public static Comparison NotEqualTo = new("!=");
        public static Comparison LessThan = new("<");
        public static Comparison LessThanOrEqualTo = new("<=");
        public static Comparison GreaterThan = new(">");
        public static Comparison GreaterThanOrEqualTo = new(">=");
        public static Comparison Like = new("LIKE");
        public static Comparison NotLike = new("NOT LIKE");
        public static Comparison In = new("IN");
        public static Comparison NotIn = new("NOT IN");
        public static Comparison IsNull = new("IS NULL");
        public static Comparison IsNotNull = new("IS NOT NULL");
        public static Comparison Between = new("BETWEEN");
        public static Comparison NotBetween = new("NOT BETWEEN");
        public static Comparison Exists = new("EXISTS");
        public static Comparison NotExists = new("NOT EXISTS");

        private readonly string SqlString { get; }

        private Comparison(string sqlString)
        {
            SqlString = sqlString;
        }

        public override string ToString()
        {
            return SqlString;
        }


        public bool Equals(Comparison other)
        {
            return string.Equals(SqlString, other.SqlString);
        }


        public static implicit operator string(Comparison c)
        {
            return c.SqlString;
        }
    }
}
