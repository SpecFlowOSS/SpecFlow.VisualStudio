using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class MsTestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string GetRunInCurrentContextCommand(bool debug)
        {
            if (debug)
                return "Test.DebugTestsInCurrentContext";

            return "Test.RunTestsInCurrentContext";
        }

        public MsTestRunnerGateway(DTE dte, IIdeTracer tracer) : base(dte, tracer)
        {
        }

        protected override int GetFeatureCodeBehindLine(TextDocument codeBehindDoc)
        {
            return GetCodeBehindLine(codeBehindDoc, 1, null);
        }
    }
}