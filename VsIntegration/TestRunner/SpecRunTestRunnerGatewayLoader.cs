using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class SpecRunTestRunnerGatewayLoader : SpecRunGatewayLoader
    {
        public SpecRunTestRunnerGatewayLoader() : base(TestRunnerTool.SpecRun)
        {
        }
    }
}