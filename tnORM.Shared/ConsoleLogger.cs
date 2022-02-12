namespace tnORM.Shared
{
    public static class ConsoleLogger
    {
        public static void LogLine(object message)
        {
            Console.WriteLine(message);
        }


        public static void LogError(string message, Exception e)
        {
            if (!string.IsNullOrEmpty(message))
            {
                LogLine(message);
            }
            LogLine(e.Message);
            if(!string.IsNullOrEmpty(e.StackTrace))
            {
                LogLine(e.StackTrace);
            }
        }
    }
}
