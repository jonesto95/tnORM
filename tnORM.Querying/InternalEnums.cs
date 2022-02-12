namespace tnORM.Querying
{
    internal enum TopType
    {
        Percent,
        Quantity
    }


    internal enum QueryChainType
    {
        UNION_ALL,
        UNION,
        EXCEPT,
        INTERSECT,
        INTERSECT_ALL
    }


    internal enum JoinType
    {
        INNER,
        LEFT,
        RIGHT,
        CROSS,
        FULL
    }

    internal enum BuildOnClause
    {
        None,
        Where,
        Join,
        Having
    }


    internal enum AnyAll
    {
        ANY,
        ALL
    }
}
