namespace tnORM.Querying
{
    public class SqlDelete
    {
        private readonly string TableAlias;
        private readonly string FromTable;
        private readonly List<SqlTableJoin> TableJoins = new();

        private BuildOnClause BuildOnClause;
        private SqlClause WhereClause;


        #region Delete Build methods

        public static SqlDelete From<T>() where T : tnORMTableBase
        {
            return From<T>(null);
        }


        public static SqlDelete From<T>(string alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            return new SqlDelete(tableInstance.DatabaseName, tableInstance.SchemaName, tableInstance.TableName, alias);
        }


        private SqlDelete(string database, string schema, string table, string tableAlias)
        {
            TableAlias = tableAlias;
            FromTable = $"{database}.[{schema}].[{table}] AS [{tableAlias}]";
        }

        #endregion


        #region WHERE methods

        public SqlDelete Where(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Where(predicate);
        }


        public SqlDelete Where(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Where(predicate);
        }


        public SqlDelete Where(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Where(predicate);
        }


        public SqlDelete Where(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Where(predicate);
        }


        public SqlDelete Where(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Where(predicate);
        }


        public SqlDelete Where(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return Where(clause);
        }


        public SqlDelete Where(SqlClause clause)
        {
            BuildOnClause = BuildOnClause.Where;
            if(WhereClause == null)
            {
                WhereClause = new();
            }
            WhereClause.And(clause);
            return this;
        }

        #endregion


        #region JOIN methods

        public SqlDelete InnerJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, null);
        }


        public SqlDelete InnerJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, alias);
        }


        public SqlDelete InnerJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.INNER, query, alias);
        }


        public SqlDelete LeftJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, null);
        }


        public SqlDelete LeftJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, alias);
        }


        public SqlDelete LeftJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.LEFT, query, alias);
        }


        public SqlDelete RightJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, null);
        }


        public SqlDelete RightJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, alias);
        }


        public SqlDelete RightJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.RIGHT, query, alias);
        }


        public SqlDelete CrossJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, null);
        }


        public SqlDelete CrossJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, alias);
        }


        public SqlDelete CrossJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.CROSS, query, alias);
        }


        private SqlDelete AddJoin<T>(JoinType joinType, string alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string table = $"{tableInstance.DatabaseName}.[{tableInstance.SchemaName}].[{tableInstance.TableName}]";
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            return AddJoin(joinType, table, alias);
        }


        private SqlDelete AddJoin(JoinType joinType, SqlSelect query, string alias)
        {
            SqlTableJoin join = new(joinType, query, alias);
            TableJoins.Add(join);
            return this;
        }


        private SqlDelete AddJoin(JoinType joinType, string table, string alias)
        {
            SqlTableJoin join = new(joinType, table, alias);
            TableJoins.Add(join);
            return this;
        }

        #endregion


        #region ON methods

        public SqlDelete On(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return On(predicate);
        }


        public SqlDelete On(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return On(predicate);
        }


        public SqlDelete On(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return On(predicate);
        }


        public SqlDelete On(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return On(predicate);
        }


        public SqlDelete On(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return On(predicate);
        }


        public SqlDelete On(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return On(clause);
        }


        public SqlDelete On(SqlClause clause)
        {
            BuildOnClause = BuildOnClause.Join;
            if (TableJoins[^1].Clause == null)
            {
                TableJoins[^1].Clause = new();
            }
            TableJoins[^1].Clause.And(clause);
            return this;
        }

        #endregion


        #region AND methods

        public SqlDelete And(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return And(predicate);
        }


        public SqlDelete And(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return And(predicate);
        }


        public SqlDelete And(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return And(predicate);
        }


        public SqlDelete And(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return And(predicate);
        }


        public SqlDelete And(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return And(predicate);
        }


        public SqlDelete And(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return And(condition);
        }


        public SqlDelete And(SqlClause clause)
        {
            switch (BuildOnClause)
            {
                case BuildOnClause.Where:
                    WhereClause.And(clause);
                    return this;

                case BuildOnClause.Join:
                    TableJoins[^1].Clause.And(clause);
                    return this;

                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion


        #region OR methods

        public SqlDelete Or(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Or(predicate);
        }


        public SqlDelete Or(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Or(predicate);
        }


        public SqlDelete Or(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Or(predicate);
        }


        public SqlDelete Or(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Or(predicate);
        }


        public SqlDelete Or(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Or(predicate);
        }


        public SqlDelete Or(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return Or(condition);
        }


        public SqlDelete Or(SqlClause clause)
        {
            switch (BuildOnClause)
            {
                case BuildOnClause.Where:
                    WhereClause.Or(clause);
                    return this;

                case BuildOnClause.Join:
                    TableJoins[^1].Clause.Or(clause);
                    return this;

                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion


        public override string ToString()
        {
            string result = $"DELETE [{TableAlias}] FROM {FromTable} ";
            if(TableJoins.Count > 0)
            {
                foreach(var tableJoin in TableJoins)
                {
                    result += $"{tableJoin} ";
                }
            }
            if(WhereClause != null)
            {
                result += $"WHERE {WhereClause}";
            }
            result = result.Trim();
            return result;
        }


        public static implicit operator string(SqlDelete s)
        {
            return s.ToString();
        }

    }
}
