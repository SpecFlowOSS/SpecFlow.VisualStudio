using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class VsTestExplorerGatewayLoader : AutoTestRunnerGatewayLoader
    {
        public VsTestExplorerGatewayLoader() : base(TestRunnerTool.VisualStudio2012)
        {
        }

        public override bool CanUse(Project project)
        {
            return true; // if loaded, can be used
        }
    }
}