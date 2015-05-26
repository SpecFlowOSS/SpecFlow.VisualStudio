using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class ReSharper6GatewayLoader : AutoTestRunnerGatewayLoader
    {
        public ReSharper6GatewayLoader()
            : base(TestRunnerTool.ReSharper)
        {
        }

        public override bool CanUse(Project project)
        {
            return GetResharperVersion() != -1;
        }

        public static int GetResharperVersion()
        {
            var reSharperAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "JetBrains.ReSharper.UnitTestFramework");
            return reSharperAssembly != null ? reSharperAssembly.GetName().Version.Major : -1;
        }
    }

    public class ReSharper6TestRunnerGateway : CommandBasedTestRunnerGateway
    {
        private readonly int _currentVersion;

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

            if (debug)
            {
                return string.Format(commandFormat, "Debug");
            }
            return string.Format(commandFormat, "Run");

        }

        public ReSharper6TestRunnerGateway(DTE dte, IIdeTracer tracer)
            : base(dte, tracer)
        {
            _currentVersion = ReSharper6GatewayLoader.GetResharperVersion();
        }
    }
}