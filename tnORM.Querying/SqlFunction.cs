using tnORM.Shared;

namespace tnORM.Querying
{
    public static class SqlFunction
    {
        public static RawSqlText Round(string fieldName, int decimalPlaces)
        {
            string sql = $"ROUND({fieldName}, {decimalPlaces})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Uppercase(string fieldName)
        {
            return Upper(fieldName);
        }


        public static RawSqlText Upper(string fieldName)
        {
            string sql = $"UPPER({fieldName})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Lowercase(string fieldName)
        {
            return Lower(fieldName);
        }


        public static RawSqlText Lower(string fieldName)
        {
            string sql = $"LOWER({fieldName})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Substring(string fieldName, int startIndex, int length)
        {
            string sql = $"SUBSTRING({fieldName}, {startIndex}, {length})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Length(string fieldName)
        {
            return Len(fieldName);
        }


        public static RawSqlText Len(string fieldName)
        {
            string sql = $"LEN({fieldName})";
            return new RawSqlText(sql);
        }


        public static RawSqlText GetDate()
        {
            string sql = $"GETDATE()";
            return new RawSqlText(sql);
        }


        public static RawSqlText GetUtcDate()
        {
            string sql = $"GETUTCDATE()";
            return new RawSqlText(sql);
        }


        public static RawSqlText Format(string fieldName, string format)
        {
            string sql = $"FORMAT({fieldName}, {format.ToSqlString()})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Format(string fieldName, string format, string culture)
        {
            string sql = $"FORMAT({fieldName}, {format.ToSqlString()}, {culture.ToSqlString()})";
            return new RawSqlText(sql);
        }
    }
}
