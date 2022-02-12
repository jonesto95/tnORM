namespace tnORM.Querying
{
    public class RawSqlText
    {
        private readonly string Sql;

        public RawSqlText(string sqlText)
        {
            Sql = sqlText;
        }


        public override string ToString()
        {
            return Sql;
        }


        public static implicit operator string(RawSqlText s)
        {
            return s.Sql;
        }
    }
}
