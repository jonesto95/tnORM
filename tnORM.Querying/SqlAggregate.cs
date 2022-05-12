namespace tnORM.Querying
{
    public static class SqlAggregate
    {
        #region COUNT Functions

        public static RawSqlText Count()
        {
            return Count("*");
        }


        public static RawSqlText CountDistinct(string fieldName)
        {
            return Count($"DISTINCT {fieldName}");
        }


        public static RawSqlText Count(string fieldName)
        {
            string sql = $"COUNT({fieldName})";
            return new RawSqlText(sql);
        }

        #endregion


        #region SUM Functions

        public static RawSqlText Sum()
        {
            return Sum("*");
        }


        public static RawSqlText SumDistinct(string fieldName)
        {
            return Sum($"DISTINCT {fieldName}");
        }


        public static RawSqlText Sum(string fieldName)
        {
            string sql = $"SUM({fieldName})";
            return new RawSqlText(sql);
        }

        #endregion


        #region MIN Functions

        public static RawSqlText Min()
        {
            return Min("*");
        }


        public static RawSqlText MinDistinct(string fieldName)
        {
            return Min($"DISTINCT {fieldName}");
        }


        public static RawSqlText Min(string fieldName)
        {
            string sql = $"MIN({fieldName})";
            return new RawSqlText(sql);
        }

        #endregion


        #region MAX Functions

        public static RawSqlText Max()
        {
            return Max("*");
        }


        public static RawSqlText MaxDistinct(string fieldName)
        {
            return Max($"DISTINCT {fieldName}");
        }


        public static RawSqlText Max(string fieldName)
        {
            string sql = $"MAX({fieldName})";
            return new RawSqlText(sql);
        }

        #endregion


        #region AVG Functions

        public static RawSqlText Avg()
        {
            return Avg("*");
        }


        public static RawSqlText AvgDistinct(string fieldName)
        {
            return Avg($"DISTINCT {fieldName}");
        }


        public static RawSqlText Avg(string fieldName)
        {
            string sql = $"AVG({fieldName})";
            return new RawSqlText(sql);
        }


        public static RawSqlText Average()
        {
            return Average("*");
        }


        public static RawSqlText AverageDistinct(string fieldName)
        {
            return Average($"DISTINCT {fieldName}");
        }


        public static RawSqlText Average(string fieldName)
        {
            string sql = $"AVG({fieldName})";
            return new RawSqlText(sql);
        }

        #endregion
    }
}
