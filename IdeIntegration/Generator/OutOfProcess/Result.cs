namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    class Result
    {
        public Result(int exitCode, string output)
        {
            ExitCode = exitCode;
            Output = output;
        }

        public int ExitCode { get; private set; }
        public string Output { get; private set; }
    }
}