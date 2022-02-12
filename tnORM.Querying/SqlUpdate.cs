using tnORM.Shared;

namespace tnORM.Querying
{
    public class SqlUpdate
    {
        private readonly string TableAlias;
        private readonly string TableName;
        private readonly Dictionary<string, string> Updates = new();

        private BuildOnClause BuildOnClause;

        private SqlClause WhereClause;
        private readonly List<SqlTableJoin> TableJoins = new();


        #region Update Build methods

        public static SqlUpdate From<T>() where T : tnORMTableBase
        {
            return From<T>(null);
        }


        public static SqlUpdate From<T>(string alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            return new SqlUpdate(tableInstance.DatabaseName, tableInstance.SchemaName, tableInstance.TableName, alias);
        }


        private SqlUpdate(string database, string schema, string table, string alias)
        {
            TableAlias = alias;
            TableName = $"{database}.[{schema}].[{table}] AS [{TableAlias}]";
        }

        #endregion


        #region WHERE methods

        public SqlUpdate Where(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Where(predicate);
        }


        public SqlUpdate Where(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Where(predicate);
        }


        public SqlUpdate Where(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Where(predicate);
        }


        public SqlUpdate Where(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Where(predicate);
        }


        public SqlUpdate Where(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Where(predicate);
        }


        public SqlUpdate Where(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return Where(clause);
        }


        public SqlUpdate Where(SqlClause clause)
        {
            BuildOnClause = BuildOnClause.Where;
            if (WhereClause == null)
            {
                WhereClause = new();
            }
            WhereClause.And(clause);
            return this;
        }

        #endregion


        #region JOIN methods

        public SqlUpdate InnerJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, null);
        }


        public SqlUpdate InnerJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, alias);
        }


        public SqlUpdate InnerJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.INNER, query, alias);
        }


        public SqlUpdate LeftJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, null);
        }


        public SqlUpdate LeftJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, alias);
        }


        public SqlUpdate LeftJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.LEFT, query, alias);
        }


        public SqlUpdate RightJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, null);
        }


        public SqlUpdate RightJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, alias);
        }


        public SqlUpdate RightJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.RIGHT, query, alias);
        }


        public SqlUpdate CrossJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, null);
        }


        public SqlUpdate CrossJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, alias);
        }


        public SqlUpdate CrossJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.CROSS, query, alias);
        }


        private SqlUpdate AddJoin<T>(JoinType joinType, string alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string table = $"{tableInstance.DatabaseName}.[{tableInstance.SchemaName}].[{tableInstance.TableName}]";
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            return AddJoin(joinType, table, alias);
        }


        private SqlUpdate AddJoin(JoinType joinType, SqlSelect query, string alias)
        {
            SqlTableJoin join = new(joinType, query, alias);
            TableJoins.Add(join);
            return this;
        }


        private SqlUpdate AddJoin(JoinType joinType, string table, string alias)
        {
            SqlTableJoin join = new(joinType, table, alias);
            TableJoins.Add(join);
            return this;
        }

        #endregion


        #region ON methods

        public SqlUpdate On(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return On(predicate);
        }


        public SqlUpdate On(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return On(predicate);
        }


        public SqlUpdate On(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return On(predicate);
        }


        public SqlUpdate On(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return On(predicate);
        }


        public SqlUpdate On(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return On(predicate);
        }


        public SqlUpdate On(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return On(clause);
        }


        public SqlUpdate On(SqlClause clause)
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

        public SqlUpdate And(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return And(predicate);
        }


        public SqlUpdate And(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return And(predicate);
        }


        public SqlUpdate And(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return And(predicate);
        }


        public SqlUpdate And(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return And(predicate);
        }


        public SqlUpdate And(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return And(predicate);
        }


        public SqlUpdate And(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return And(condition);
        }


        public SqlUpdate And(SqlClause clause)
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

        public SqlUpdate Or(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Or(predicate);
        }


        public SqlUpdate Or(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Or(predicate);
        }


        public SqlUpdate Or(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Or(predicate);
        }


        public SqlUpdate Or(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Or(predicate);
        }


        public SqlUpdate Or(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Or(predicate);
        }


        public SqlUpdate Or(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return Or(condition);
        }


        public SqlUpdate Or(SqlClause clause)
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


        public SqlUpdate Set(SqlField sqlField, object value)
        {
            Updates[sqlField] = value.ToSqlString();
            return this;
        }


        public SqlUpdate Set(string fieldname, object value)
        {
            Updates[fieldname] = value.ToSqlString();
            return this;
        }


        public object this[string fieldName]
        {
            set
            {
                Set(fieldName, value);
            }
        }


        public override string ToString()
        {
            string result = $"UPDATE [{TableAlias}] SET ";
            if(Updates.Count == 0)
            {
                throw new EmptyUpdateStatementException();
            }
            foreach(var keyValuePair in Updates)
            {
                result += $"{keyValuePair.Key} = {keyValuePair.Value}, ";
            }
            result = result[..^2];
            result += $" FROM {TableName}";
            foreach(var join in TableJoins)
            {
                result += $" {join}";
            }
            if(WhereClause != null)
            {
                result += $" WHERE {WhereClause}";
            }
            result = result.Trim();
            return result;
        }


        public static implicit operator string(SqlUpdate s)
        {
            return s.ToString();
        }
    }


    public class EmptyUpdateStatementException : Exception
    {
        public EmptyUpdateStatementException()
            : base("The UPDATE statment does not set any values") { }
    }
}
