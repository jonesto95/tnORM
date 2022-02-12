using tnORM.Querying;

namespace tnORM.Tests
{
    internal abstract class TestRunnerBase
    {
        protected SqlField BaseField { get; } = new("BaseTable", "BaseField");

        private bool EndOnError { get; set; }


        public void Run(bool endOnError)
        {
            int i = 0;
            EndOnError = endOnError;
            while (true)
            {
                try
                {
                    i++;
                    string methodName = $"Test{i:000}";
                    var method = GetType().GetMethod(methodName);
                    if(method == null)
                    {
                        break;
                    }

                    LogLine();
                    LogLine($"Running test {method}");
                    method.Invoke(this, null);
                }
                catch (Exception e)
                {
                    HandleError(e);
                }
            }
        }


        private void HandleError(Exception error)
        {
            if (!EndOnError)
            {
                return;
            }

            if(error is FailedTestCaseException)
            {
                throw error;
            }
            throw new FailedTestCaseException(error);
        }


        public void LogLine()
        {
            LogLine(string.Empty);
        }


        public void LogLine(object message)
        {
            Console.WriteLine(message);
        }


        public void AssertEqual(string resultValue, string expectedValue)
        {
            if(!string.Equals(resultValue, expectedValue))
            {
                throw new UnequalValuesException(resultValue, expectedValue);
            }
        }


        public void LogRunnerStart()
        {
            LogLine();
            LogLine($"----- Starting {GetType().Name} -----");
        }


        public object GetRandomValue()
        {
            var random = new Random();
            double randomValue = random.NextDouble();
            if (randomValue < 0.33)
            {
                return GetRandomStringValue();
            }
            if (randomValue < 0.66)
            {
                return GetRandomIntValue();
            }
            return GetRandomBooleanValue();
        }


        public string GetRandomStringValue()
        {
            var random = new Random();
            string result = string.Empty;
            string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ'%";
            for (int i = 0; i < 10; i++)
            {
                result += charPool[random.Next(charPool.Length)];
            }
            return result;
        }


        public int GetRandomIntValue()
        {
            var random = new Random();
            return random.Next(999999);
        }


        public bool GetRandomBooleanValue()
        {
            var random = new Random();
            return (random.NextDouble() > 0.5);
        }


        public DateTime GetRandomDateTimeValue()
        {
            var now = DateTime.Now;
            int minusDays = -1 * (GetRandomIntValue() / 100);
            return now.AddDays(minusDays);
        }
    }
}
