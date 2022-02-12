using Microsoft.Extensions.Configuration;

namespace tnORM.Shared
{
    public static class tnORMConfig
    {
        private static string configurationFilePath;

        private static IConfiguration Configuration
        {
            get
            {
                if (configurationInstance == null)
                {
                    LoadConfigurationFromJson("tnORMSettings.json");
                }
                return configurationInstance;
            }
        }
        private static IConfiguration configurationInstance;


        /// <summary>
        /// Load a JSON configuration file from a file path
        /// </summary>
        public static void LoadConfigurationFromJson(string filePath)
        {
            try
            {
                filePath = Path.GetFullPath(filePath);
                configurationFilePath = filePath;
                configurationInstance = new ConfigurationBuilder()
                    .AddJsonFile(filePath)
                    .Build();
            }
            catch (FileNotFoundException e)
            {
                throw new ConfigurationFileNotFoundException(filePath, e);
            }
        }


        /// <summary>
        /// Retrieves a string value from the configuration file at the specified path
        /// </summary>
        public static string GetString(string key)
        {
            string result = Configuration[key];
            if (result == null)
            {
                throw new ConfigurationNotFoundException(configurationFilePath, key);
            }
            return result;
        }


        /// <summary>
        /// Retrieves a strirng value from the configuration file at the specified path.
        /// If a value does not exist, this method returns null.
        /// </summary>
        public static string TryGetString(string key)
        {
            return Configuration[key];
        }


        /// <summary>
        /// Retrieves a string array from the configuration file at the specified path
        /// </summary>
        public static string[] TryGetConfigurationArray(string key)
        {
            return Configuration.GetSection(key).Get<string[]>();
        }


        public static string GetDatabaseName(string databaseName)
        {
            return TryGetString($"DatabaseOverrides:{databaseName}") ?? databaseName;
        }
    }


    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string configFile, string configKey)
            : base($"Configuration {configKey} not found in configurationFile {configFile}") { }

        public ConfigurationNotFoundException(string configFile, string configKey, Exception innerException)
            : base($"Configuration {configKey} not found in configurationFile {configFile}", innerException) { }
    }


    public class ConfigurationFileNotFoundException : Exception
    {
        public ConfigurationFileNotFoundException(string filePath)
            : base($"The configuration file {filePath} was not found.") { }

        public ConfigurationFileNotFoundException(string filePath, Exception innerException)
            : base($"The configuration file {filePath} was not found.", innerException) { }
    }
}