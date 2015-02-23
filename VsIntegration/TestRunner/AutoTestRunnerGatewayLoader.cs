using BoDi;
using EnvDTE;
using System;
using System.Linq;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public abstract class AutoTestRunnerGatewayLoader
    {
        private readonly TestRunnerTool tool;

        protected AutoTestRunnerGatewayLoader(TestRunnerTool tool)
        {
            this.tool = tool;
        }

        public abstract bool CanUse(Project project);

        public ITestRunnerGateway CreateTestRunner(IObjectContainer container)
        {
            return container.Resolve<ITestRunnerGateway>(tool.ToString());
        }
    }
}
