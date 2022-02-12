using System.Reflection;

namespace tnORM.Shared
{
    public static class tnORMShared
    {
        public const string UpgradeTableName = "tnORMUpgradeScriptRun";
        
        private static readonly DateTime Epoch = Convert.ToDateTime("1/1/1900");

        public static decimal NewGuid()
        {
            Random random = new();
            var createdDate = DateTime.Now;
            string idSuffix = $"{random.Next(16777216, 20000000)}";
            double totalDays = (createdDate - Epoch).TotalDays;
            string createdDateString = string.Format("{0:0.00000000}", totalDays).Replace(".", string.Empty);
            return decimal.Parse(createdDateString + idSuffix);
        }

        public static T GetProperty<T>(this object input, string propertyName)
        {
            object result = input;
            string[] properties = propertyName.Split('.');
            var type = result.GetType();
            foreach (string prop in properties)
            {
                var propData = GetPropertyInfo(type, prop);
                result = propData.GetValue(result, null);
            }
            return (T)result;
        }


        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property == null)
            {
                throw new NonexistentPropertyException(type.Name, propertyName);
            }
            return property;
        }


        public static T SetProperty<T>(this T input, string propertyName, object value)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var property = GetPropertyInfo(input.GetType(), propertyName);
            if (property.PropertyType == typeof(bool) && value is int valueInt)
            {
                value = (valueInt == 1);
            }
            property.SetValue(input, value);
            return input;
        }

        public static string BuildInList(object[] values)
        {
            string result = string.Empty;
            foreach (object value in values)
            {
                result += value.ToString() + ", ";
            }
            result = result[..^2];
            return result;
        }


        public static string ToSqlString(this object input)
        {
            if (input == null)
            {
                return "NULL";
            }
            else if (input is string inputString)
            {
                inputString = inputString.Replace("'", "''");
                return $"'{inputString}'";
            }
            else if (input is bool)
            {
                bool inputBool = (bool)input;
                return (inputBool ? "1" : "0");
            }
            else if (input is bool?)
            {
                bool? inputBool = (bool?)input;
                return (inputBool.Value ? "1" : "0");
            }
            else if (input is DateTime)
            {
                DateTime inputDate = (DateTime)input;
                string result = inputDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
                return $"'{result}'";
            }
            else if (input is DateTime?)
            {
                DateTime? inputDate = (DateTime?)input;
                string result = inputDate.Value.ToString("MM/dd/yyyy HH:mm:ss.fff");
                return $"'{result}'";
            }
            else if (input is Guid)
            {
                return $"'{input}'";
            }
            else
            {
                return input.ToString();
            }
        }
    }

    public class NonexistentPropertyException : Exception
    {
        public NonexistentPropertyException(string objectType, string propertyName)
            : base($"Object type {objectType} does not have a property named \"{propertyName}\"")
        { }
    }
}
