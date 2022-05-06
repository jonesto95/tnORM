using tnORM.Tests;
using tnORM.Tests.Runners;

TestRunnerBase[] runners =
{
    new SqlFieldTestRunner()
    , new SqlPredicateTestRunner()
    , new SqlClauseTestRunner()
    , new CaseTestRunner()
    , new SqlSelectTestRunner()
    , new SqlUpdateTestRunner()
    , new SqlDeleteTestRunner()
    , new SqlTableRunner()
};


bool endTestRunOnError = true;

foreach(var runner in runners)
{
    try
    {
        runner.LogRunnerStart();
        runner.Run(endTestRunOnError);
        runner.LogLine();
    }
    catch (FailedTestCaseException)
    {
        throw;
    }
}
Console.WriteLine("TESTS RAN SUCCESSFULLY");
Thread.Sleep(3000);


public class FailedTestCaseException : Exception
{
    public FailedTestCaseException()
        : base("A test case has failed. Terminating process.") { }


    public FailedTestCaseException(Exception e) 
        : base("A test case has failed. Terminating process.", e) { }
}


public class UnequalValuesException : Exception
{
    public UnequalValuesException(object resultValue, object expectedValue)
        : base($"Resulting value \"{resultValue}\" is not equal to expected value \"{expectedValue}\"")
    { }
}


public class NullValueException : Exception
{
    public NullValueException()
        : base("Value is null when a value was expected")
    { }
}