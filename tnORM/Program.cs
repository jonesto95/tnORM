using tnORM.Shared;

namespace tnORM
{
    class Program
    {
        private static bool CreateDatabase { get; set; }
        private static string? ReferenceDatabase { get; set; }
        private static string? DatabaseName { get; set; }
        private static string? Schemas { get; set; }
        private static bool PerformClassGeneration { get; set; } = true;
        private static bool PerformScriptExecution { get; set; } = true;
        private static bool PerformCodeCompilation { get; set; } = true;
        private static string[]? Arguments { get; set; }
        private static bool PauseOnEnd { get; set; } = false;

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

                    if(arg.StartsWith("refdb:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReferenceDatabase = arg[(arg.IndexOf(':') + 1)..];
                        continue;
                    }
                    if(arg.StartsWith("schema:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Schemas = arg[(arg.IndexOf(':') + 1)..];
                        continue;
                    }
                    if(arg.StartsWith("dbname:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DatabaseName = arg[(arg.IndexOf(':') + 1)..];
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
                    if(arg.Equals("-pause", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PauseOnEnd = true;
                        continue;
                    }
                    throw new UnknownArgumentException(arg);
                }
            }
        }


        private static void BeginProcessing()
        {
            if (string.IsNullOrEmpty(Schemas))
            {
                Console.WriteLine("No schemas specified. Defaulting to schema 'dbo'");
                Schemas = "dbo";
            }
            if (string.IsNullOrEmpty(DatabaseName))
            {
                Console.WriteLine($"Database name not specified. Defaulting to {ReferenceDatabase}");
                DatabaseName = ReferenceDatabase;
            }
            if(PerformScriptExecution)
            {
                DbScriptExecutor.Run(ReferenceDatabase);
            }
            if (PerformClassGeneration)
            {
                ORMCodeGenerator.Run(ReferenceDatabase, DatabaseName, Schemas);
            }
            if (PerformCodeCompilation)
            {
                ORMCodeCompiler.Run(ReferenceDatabase, DatabaseName, Schemas);
            }
            if (PauseOnEnd)
            {
                ConsoleLogger.LogLine("Program complete. Press Enter to exit");
            }
            Console.ReadLine();
        }
    }


    public class UnknownArgumentException : Exception
    {
        public UnknownArgumentException(string arg)
            : base($"The argument \"{arg}\" was not recognized") { }
    }
}