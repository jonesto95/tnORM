namespace tnORM.Querying
{
    sealed class QueryChainLink
    {
        public QueryChainType ChainType { get; private set; }

        public SqlSelect Query { get; private set; }


        public QueryChainLink(SqlSelect query, QueryChainType chainType)
        {
            ChainType = chainType;
            Query = query;
        }


        public void Nolock()
        {
            Nolock(true);
        }


        public void Nolock(bool useNolock)
        {
            Query.Nolock(useNolock);
        }
    }
}
