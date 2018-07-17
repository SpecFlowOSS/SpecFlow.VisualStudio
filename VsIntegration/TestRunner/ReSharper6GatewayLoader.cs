using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;

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
}