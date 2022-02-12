namespace tnORM.Querying
{
    public static class DefaultExpressions
    {
        public const string GetUtcDate = "GETUTCDATE()";
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKey : Attribute { }


    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseColumn : Attribute { }


    [AttributeUsage(AttributeTargets.Property)]
    public class Identity : Attribute { }
}
