using tnORM.Shared;

namespace tnORM
{
    class Program
    {
        private static bool CreateDatabase { get; set; }
        private static string? Database { get; set; }
        private static string? Schemas { get; set; }
        private static bool PerformClassGeneration { get; set; } = true;
        private static bool PerformScriptExecution { get; set; } = true;
        private static bool PerformCodeCompilation { get; set; } = true;
        private static string[]? Arguments { get; set; }

        static void Main(string[] args)
        {
            CreateDatabase = false;
            Arguments = args;
            ParseArguments();
            BeginProcessing();
        }


        private static void ParseArguments()
        {
            if(Arguments == null)
            {
                return;
            }
            foreach(string arg in Arguments)
            {
                if(!string.IsNullOrEmpty(arg))
                {

                    if(arg.StartsWith("database:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Database = arg[(arg.IndexOf(':') + 1)..];
                        continue;
                    }
                    if(arg.StartsWith("schema:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Schemas = arg[(arg.IndexOf(':') + 1)..];
                        continue;
                    }
                    if (arg.Equals("-noscript", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PerformScriptExecution = false;
                        continue;
                    }
                    if(arg.Equals("-nogen", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PerformClassGeneration = false;
                        continue;
                    }
                    if (arg.Equals("-nocompile", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PerformCodeCompilation = false;
                        continue;
                    }
                    throw new UnknownArgumentException(arg);
                }
            }
        }


        private static void BeginProcessing()
        {
            if(PerformScriptExecution)
            {
                DbScriptExecutor.Run(Database);
            }
            if (PerformClassGeneration)
            {
                ORMCodeGenerator.Run(Database, Schemas);
            }
            if (PerformCodeCompilation)
            {
                ORMCodeCompiler.Run(Database, Schemas);
            }
            ConsoleLogger.LogLine("Program complete. Press Enter to exit");
            Console.ReadLine();
        }
    }


    public class UnknownArgumentException : Exception
    {
        public UnknownArgumentException(string arg)
            : base($"The argument \"{arg}\" was not recognized") { }
    }
}