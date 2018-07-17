using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public interface ITestRunnerGatewayProvider
    {
        ITestRunnerGateway GetTestRunnerGateway(TestRunnerTool? runnerTool = null);
    }
}