using tnORM.Shared;

namespace tnORM.Querying
{
    public class SqlSelect
    {
        private bool SelectTopPercentage { get; set; }
        private bool SelectDistinct { get; set; }
        private bool UseNolock { get; set; } = false;
        private int? SelectTop { get; set; }
        private bool SelectTopWithTies { get; set; } = false;
        private int? OffsetRows { get; set; }
        private int? FetchNextRowsOnly { get; set; }
        private BuildOnClause BuildOnClause { get; set; }
        private string FromTable { get; set; }
        private List<string> SelectList { get; set; } = new() { "*" };
        private List<string> GroupByList { get; set; } = new();
        private List<string> OrderByList { get; set; } = new();
        private SqlClause WhereClause { get; set; }
        private SqlClause HavingClause { get; set; }
        private List<SqlTableJoin> TableJoins { get; set; } = new();
        private List<QueryChainLink> ChainedQueries { get; set; } = new();
        private bool selectAll = false;

        #region Select build methods

        public static SqlSelect From<T>() where T : tnORMTableBase
        {
            return From<T>(null);
        }


        public static SqlSelect From<T>(string tableAlias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            if (string.IsNullOrEmpty(tableAlias))
            {
                tableAlias = tableInstance.TableAlias;
            }
            return new SqlSelect(tableInstance.DatabaseName, tableInstance.SchemaName, tableInstance.TableName, tableAlias);
        }


        private SqlSelect(string database, string schema, string table, string tableAlias)
        {
            FromTable = $"{database}.[{schema}].[{table}] AS [{tableAlias}]";
        }

        #endregion


        #region SELECT methods

        public SqlSelect Select(params string[] columns)
        {
            if (SelectList.Count == 1 && SelectList[0].EndsWith("*"))
            {
                SelectList.Clear();
            }
            SelectList.AddRange(columns);
            return this;
        }


        public SqlSelect Select(params object[] columns)
        {
            if (SelectList.Count == 1 && SelectList[0].EndsWith("*"))
            {
                SelectList.Clear();
            }
            foreach (object obj in columns)
            {
                Select(obj.ToSqlString());
            }
            return this;
        }


        public SqlSelect Select(Case caseStatement)
        {
            if (SelectList.Count == 1 && SelectList[0].EndsWith("*"))
            {
                SelectList.Clear();
            }
            SelectList.Add(caseStatement);
            return this;
        }


        public SqlSelect Select<T>() where T : tnORMTableBase
        {
            return Select<T>(null);
        }


        public SqlSelect Select<T>(string alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            SelectList.Clear();
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            SelectList.Add($"{alias}.*");
            return this;
        }


        public SqlSelect ResetSelect()
        {
            SelectList = new() { "*" };
            return this;
        }


        #region SELECT adjacent methods

        public SqlSelect Nolock()
        {
            return Nolock(true);
        }


        public SqlSelect Nolock(bool useNolock)
        {
            UseNolock = useNolock;
            foreach (var join in TableJoins)
            {
                join.Nolock(useNolock);
            }
            foreach (var query in ChainedQueries)
            {
                query.Nolock(useNolock);
            }
            return this;
        }


        public SqlSelect Top(int amount)
        {
            return Top(amount, TopType.Quantity, false);
        }


        public SqlSelect Top(int amount, bool withTies)
        {
            return Top(amount, TopType.Quantity, withTies);
        }


        public SqlSelect TopPercent(int amount)
        {
            return Top(amount, TopType.Percent, false);
        }


        public SqlSelect TopPercent(int amount, bool withTies)
        {
            return Top(amount, TopType.Percent, withTies);
        }


        private SqlSelect Top(int amount, TopType topType, bool withTies)
        {
            if (amount < 0)
            {
                throw new InvalidTopAmountException();
            }
            if (topType == TopType.Percent && amount > 100)
            {
                throw new InvalidTopAmountException();
            }
            SelectTop = amount;
            SelectTopPercentage = (topType == TopType.Percent);
            SelectTopWithTies = withTies;
            return this;
        }


        public SqlSelect Distinct()
        {
            return Distinct(true);
        }


        public SqlSelect Distinct(bool useDistinct)
        {
            SelectDistinct = useDistinct;
            return this;
        }


        public SqlSelect SelectAll()
        {
            return SelectAll(true);
        }


        public SqlSelect SelectAll(bool selectAll)
        {
            selectAll = selectAll;
            return this;
        }

        #endregion

        #endregion


        #region WHERE methods

        public SqlSelect Where(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Where(predicate);
        }


        public SqlSelect Where(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Where(predicate);
        }


        public SqlSelect Where(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Where(predicate);
        }


        public SqlSelect Where(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Where(predicate);
        }


        public SqlSelect Where(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Where(predicate);
        }


        public SqlSelect Where(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return Where(clause);
        }


        public SqlSelect Where(SqlClause clause)
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

        public SqlSelect InnerJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, null);
        }


        public SqlSelect InnerJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.INNER, alias);
        }


        public SqlSelect InnerJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.INNER, query, alias);
        }


        public SqlSelect LeftJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, null);
        }


        public SqlSelect LeftJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.LEFT, alias);
        }


        public SqlSelect LeftJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.LEFT, query, alias);
        }


        public SqlSelect RightJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, null);
        }


        public SqlSelect RightJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.RIGHT, alias);
        }


        public SqlSelect RightJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.RIGHT, query, alias);
        }


        public SqlSelect CrossJoin<T>() where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, null);
        }


        public SqlSelect CrossJoin<T>(string alias) where T : tnORMTableBase
        {
            return AddJoin<T>(JoinType.CROSS, alias);
        }


        public SqlSelect CrossJoin(SqlSelect query, string alias)
        {
            return AddJoin(JoinType.CROSS, query, alias);
        }


        private SqlSelect AddJoin<T>(JoinType joinType, string? alias) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string table = $"{tableInstance.DatabaseName}.[{tableInstance.SchemaName}].[{tableInstance.TableName}]";
            if (string.IsNullOrEmpty(alias))
            {
                alias = tableInstance.TableAlias;
            }
            return AddJoin(joinType, table, alias);
        }


        private SqlSelect AddJoin(JoinType joinType, SqlSelect query, string alias)
        {
            query.Nolock(UseNolock);
            SqlTableJoin join = new(joinType, query, alias);
            join.Nolock(UseNolock);
            TableJoins.Add(join);
            return this;
        }


        private SqlSelect AddJoin(JoinType joinType, string table, string alias)
        {
            SqlTableJoin join = new(joinType, table, alias);
            join.Nolock(UseNolock);
            TableJoins.Add(join);
            return this;
        }

        #endregion


        #region ON methods

        public SqlSelect On(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return On(predicate);
        }


        public SqlSelect On(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return On(predicate);
        }


        public SqlSelect On(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return On(predicate);
        }


        public SqlSelect On(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return On(predicate);
        }


        public SqlSelect On(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return On(predicate);
        }


        public SqlSelect On(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return On(clause);
        }


        public SqlSelect On(SqlClause clause)
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


        #region GROUP BY and HAVING methods

        public SqlSelect GroupBy(params string[] columns)
        {
            GroupByList.AddRange(columns);
            return this;
        }


        public SqlSelect GroupBy(params object[] columns)
        {
            foreach (var obj in columns)
            {
                GroupByList.Add(obj.ToSqlString());
            }
            return this;
        }


        public SqlSelect Having(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Having(predicate);
        }


        public SqlSelect Having(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Having(predicate);
        }


        public SqlSelect Having(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Having(predicate);
        }


        public SqlSelect Having(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Having(predicate);
        }


        public SqlSelect Having(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Having(predicate);
        }


        public SqlSelect Having(SqlPredicate predicate)
        {
            SqlClause clause = new(predicate);
            return Having(clause);
        }


        public SqlSelect Having(SqlClause clause)
        {
            BuildOnClause = BuildOnClause.Having;
            if (HavingClause == null)
            {
                HavingClause = new();
            }
            HavingClause.And(clause);
            return this;
        }

        #endregion


        #region ORDER BY methods

        public SqlSelect OrderBy(string columnName)
        {
            return OrderBy(columnName, false);
        }


        public SqlSelect OrderBy(string columnName, bool descending)
        {
            return AddOrderBy(columnName, descending);
        }


        public SqlSelect OrderBy(int columnIndex)
        {
            return OrderBy(columnIndex, false);
        }


        public SqlSelect OrderBy(int columnIndex, bool descending)
        {
            return AddOrderBy(columnIndex.ToSqlString(), descending);
        }


        private SqlSelect AddOrderBy(string name, bool descending)
        {
            if (descending)
            {
                name += $" DESC";
            }
            OrderByList.Add(name);
            return this;
        }

        #endregion


        #region AND methods

        public SqlSelect And(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return And(predicate);
        }


        public SqlSelect And(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return And(predicate);
        }


        public SqlSelect And(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return And(predicate);
        }


        public SqlSelect And(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return And(predicate);
        }


        public SqlSelect And(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return And(predicate);
        }


        public SqlSelect And(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return And(condition);
        }


        public SqlSelect And(SqlClause clause)
        {
            switch (BuildOnClause)
            {
                case BuildOnClause.Where:
                    WhereClause.And(clause);
                    return this;

                case BuildOnClause.Join:
                    TableJoins[^1].Clause.And(clause);
                    return this;

                case BuildOnClause.Having:
                    HavingClause.And(clause);
                    return this;

                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion


        #region OR methods

        public SqlSelect Or(object value1, Comparison comparison)
        {
            SqlPredicate predicate = new(value1, comparison);
            return Or(predicate);
        }


        public SqlSelect Or(object value1, Comparison comparison, object value2)
        {
            SqlPredicate predicate = new(value1, comparison, value2);
            return Or(predicate);
        }


        public SqlSelect Or(object value1, Comparison comparison, object value2, object value3)
        {
            SqlPredicate predicate = new(value1, comparison, value2, value3);
            return Or(predicate);
        }


        public SqlSelect Or(object value1, Comparison comparison, params object[] values)
        {
            SqlPredicate predicate = new(value1, comparison, values);
            return Or(predicate);
        }


        public SqlSelect Or(Comparison comparison, SqlSelect query)
        {
            SqlPredicate predicate = new(comparison, query);
            return Or(predicate);
        }


        public SqlSelect Or(SqlPredicate predicate)
        {
            SqlClause condition = new(predicate);
            return Or(condition);
        }


        public SqlSelect Or(SqlClause clause)
        {
            switch (BuildOnClause)
            {
                case BuildOnClause.Where:
                    WhereClause.Or(clause);
                    return this;

                case BuildOnClause.Join:
                    TableJoins[^1].Clause.Or(clause);
                    return this;

                case BuildOnClause.Having:
                    HavingClause.Or(clause);
                    return this;

                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion


        #region Query Chaining methods

        public SqlSelect Union(SqlSelect query)
        {
            return AddQueryToChain(query, QueryChainType.UNION);
        }


        public SqlSelect UnionAll(SqlSelect query)
        {
            return AddQueryToChain(query, QueryChainType.UNION_ALL);
        }


        public SqlSelect Except(SqlSelect query)
        {
            return AddQueryToChain(query, QueryChainType.EXCEPT);
        }


        public SqlSelect Intersect(SqlSelect query)
        {
            return AddQueryToChain(query, QueryChainType.INTERSECT);
        }


        public SqlSelect IntersectAll(SqlSelect query)
        {
            return AddQueryToChain(query, QueryChainType.INTERSECT_ALL);
        }


        private SqlSelect AddQueryToChain(SqlSelect query, QueryChainType queryChainType)
        {
            query.Nolock(UseNolock);
            QueryChainLink chainLink = new(query, queryChainType);
            ChainedQueries.Add(chainLink);
            return this;
        }

        #endregion


        #region OFFSET and FETCH methods

        public SqlSelect Offset(int? amount)
        {
            OffsetRows = amount;
            return this;
        }


        public SqlSelect FetchNextOnly(int? amount)
        {
            OffsetRows = OffsetRows.GetValueOrDefault(0);
            FetchNextRowsOnly = amount;
            return this;
        }

        #endregion


        public override string ToString()
        {
            // Build SELECT
            string result = "SELECT ";
            if (SelectTop.HasValue)
            {
                result += $"TOP {SelectTop} ";
                if(SelectTopPercentage)
                {
                    result += "PERCENT ";
                }
                if (SelectTopWithTies)
                {
                    result += "WITH TIES ";
                }
            }
            if (selectAll)
            {
                result += "ALL ";
            }
            if (SelectDistinct)
            {
                result += "DISTINCT ";
            }
            result += SelectList[0];
            for(int i = 1; i < SelectList.Count; i++)
            {
                result += $", {SelectList[i]}";
            }

            // Build FROM statement
            result += $" FROM {FromTable} ";
            if (UseNolock)
            {
                result += "(NOLOCK) ";
            }

            // Build JOINs
            if(TableJoins.Count > 0)
            {
                foreach(var tableJoin in TableJoins)
                {
                    result += $"{tableJoin} ";
                }
            }

            // Build WHERE clause
            if(WhereClause != null)
            {
                result += $"WHERE {WhereClause} ";
            }

            // Build GROUP BY
            if(GroupByList.Count > 0)
            {
                result += $"GROUP BY {GroupByList[0]} ";
                for(int i = 1; i < GroupByList.Count; i++)
                {
                    result += $", {GroupByList[i]}";
                }

                // Build HAVING
                if(HavingClause != null)
                {
                    result += $"HAVING {HavingClause} ";
                }
            }

            // Build ORDER BY
            if(OrderByList.Count > 0)
            {
                result += $"ORDER BY {OrderByList[0]}";
                for(int i = 1; i < OrderByList.Count; i++)
                {
                    result += $", {OrderByList[i]}";
                }
                result += " ";

                if (OffsetRows.HasValue)
                {
                    result += $"OFFSET {OffsetRows} ROWS ";
                    if (FetchNextRowsOnly.HasValue)
                    {
                        result += $"FETCH NEXT {FetchNextRowsOnly} ROWS ONLY ";
                    }
                }
            }

            // Query Chaining
            if(ChainedQueries.Count > 0)
            {
                string chainString = string.Empty;
                foreach(var link in ChainedQueries)
                {
                    chainString = link.ChainType.ToString().Replace("_", " ");
                    result += $"{chainString} ({link.Query.Nolock(UseNolock)}";
                }
            }

            result = result.Trim();
            return result;
        }


        public static implicit operator string(SqlSelect s)
        {
            return s.ToString();
        }
    }


    public class InvalidTopAmountException : Exception
    {
        public InvalidTopAmountException()
            : base("The amount passed in for the TOP directive is invalid") { }
    }
}
