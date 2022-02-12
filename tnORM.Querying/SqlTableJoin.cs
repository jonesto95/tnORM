namespace tnORM.Querying
{
    internal class SqlTableJoin
    {
        public SqlClause Clause { get; set; }

        private bool UseNolock { get; set; } = false;
        private string Alias { get; set; }
        private JoinType JoinType { get; set; }

        private readonly string JoinTable;
        private readonly SqlSelect JoinQuery;


        #region Constructors

        public SqlTableJoin(JoinType joinType, string table)
        {
            JoinTable = table;
            Initialize(joinType, null);
        }


        public SqlTableJoin(JoinType joinType, string table, string alias)
        {
            JoinTable = table;
            Initialize(joinType, alias);
        }


        public SqlTableJoin(JoinType joinType, SqlSelect query, string alias)
        {
            JoinQuery = query;
            Initialize(joinType, alias);
        }


        private void Initialize(JoinType joinType, string alias)
        {
            JoinType = joinType;
            Alias = alias;
        }

        #endregion


        public void Nolock()
        {
            Nolock(true);
        }


        public void Nolock(bool useNolock)
        {
            UseNolock = useNolock;
            if(JoinQuery != null)
            {
                JoinQuery.Nolock(useNolock);
            }
        }


        public override string ToString()
        {
            string result = $"{JoinType} JOIN ";
            if(JoinQuery != null)
            {
                result += $"({JoinQuery}) AS [{Alias}]";
            }
            else
            {
                result += JoinTable;
                if(Alias != null)
                {
                    result += $" AS [{Alias}]";
                }
            }
            if(UseNolock && JoinQuery == null)
            {
                result += $" (NOLOCK)";
            }
            if(JoinType != JoinType.CROSS)
            {
                result += $" ON {Clause}";
            }
            return result;
        }
    }
}
