using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class ReSharper6TestRunnerGateway : CommandBasedTestRunnerGateway
    {
        private const int Resharper2017 = 108;

        private readonly int _currentVersion;

        private string CommandToRun(bool debug)
        {
            if (debug)
            {
                return "Debug"; 
            }

            return _currentVersion >= Resharper2017 ? "RunFrom" : "Run";
        }

        protected override string GetRunInCurrentContextCommand(bool debug)
        {
            string commandFormat;
            if (_currentVersion < 6)
            {
                commandFormat = "ReSharper.ReSharper_UnitTest_Context{0}";
            }
            else if (_currentVersion < 9)
            {
                commandFormat = "ReSharper.ReSharper_ReSharper_UnitTest_{0}Context";
            }
            else
            {
                commandFormat = "ReSharper.ReSharper_UnitTest{0}Context";
            }

            return string.Format(commandFormat, CommandToRun(debug));
        }

        public ReSharper6TestRunnerGateway(DTE dte, IIdeTracer tracer)
            : base(dte, tracer)
        {
            _currentVersion = ReSharper6GatewayLoader.GetResharperVersion();
        }
    }
}