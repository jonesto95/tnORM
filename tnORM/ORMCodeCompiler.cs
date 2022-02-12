using System.Diagnostics;
using tnORM.Shared;

namespace tnORM
{
    public static class ORMCodeCompiler
    {
        public static void Run(string database, string schemas)
        {
            string[] schemaList = schemas.Split(',');
            foreach(string schema in schemaList)
            {
                try
                {
                    string outputDirectory = tnORMConfig.GetString("CodeOutputDirectory");
                    string sharedDllDirectory = Path.Combine(outputDirectory, "SharedDlls");
                    Directory.CreateDirectory(sharedDllDirectory);
                    outputDirectory += $"/{database}.{schema}";
                    if(Directory.GetFiles(outputDirectory, "*.csproj").Length == 0)
                    {
                        WriteProjectFile(outputDirectory, schema);
                    }
                    string projectFilePath = Directory.GetFiles(outputDirectory, "*.csproj")[0];
                    ConsoleLogger.LogLine($"Buildling {projectFilePath}...");
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo("cmd", $"/c dotnet build {projectFilePath}")
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        }
                    };
                    process.Start();
                    string processResult = process.StandardOutput.ReadToEnd();
                    if(process.ExitCode == 0)
                    {
                        string[] copyDirectories = tnORMConfig.TryGetConfigurationArray("DllCopyDirectories");
                        if(copyDirectories != null)
                        {
                            string fileName = $"tnORM.Model.{schema}.dll";
                            string outputDll = outputDirectory + $"/bin/Debug/net6.0/{fileName}";
                            foreach(string directory in copyDirectories)
                            {
                                Directory.CreateDirectory(directory);
                                ConsoleLogger.LogLine($"Copying file {outputDll} to {directory}");
                                File.Copy(outputDll, Path.Combine(directory, fileName), true);
                            }
                        }
                    }
                    else
                    {
                        ConsoleLogger.LogLine("ERROR(S) IN BUILD");
                        ConsoleLogger.LogLine(processResult);
                        ConsoleLogger.LogLine(string.Empty);
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    ConsoleLogger.LogError($"No project file found in /{database}.{schema} directory", e);
                }
            }
        }


        private static void WriteProjectFile(string outputDirectory, string schema)
        {
            string fileContent =
                "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n" +
                "  <PropertyGroup>\r\n" +
                "    <TargetFramework>net6.0</TargetFramework>\r\n" +
                "    <ImplicitUsings>enable</ImplicitUsings>\r\n" +
                "    <Nullable>enable</Nullable>\r\n" +
                "  </PropertyGroup>\r\n" +
                "  <ItemGroup>\r\n" +
                "    <Reference Include=\"tnORM.Shared\">\r\n" +
                "        <HintPath>..\\SharedDlls\\tnORM.Shared.dll</HintPath>\r\n" +
                "    </Reference>\r\n" +
                "    <Reference Include=\"tnORM.Querying\">\r\n" +
                "        <HintPath>..\\SharedDlls\\tnORM.Querying.dll</HintPath>\r\n" +
                "    </Reference>\r\n" +
                "  </ItemGroup>\r\n" +
                "</Project>";
            string fileName = $"tnORM.Model.{schema}.csproj";
            outputDirectory += $"/{fileName}";
            File.WriteAllText(outputDirectory, fileContent);
        }
    }
}