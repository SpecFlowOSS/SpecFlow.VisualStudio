using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public abstract class SpecRunGatewayLoader : AutoTestRunnerGatewayLoader
    {
        protected SpecRunGatewayLoader(TestRunnerTool tool) : base(tool)
        {
        }

        public override bool CanUse(Project project)
        {
            return IsSpecRunProject(project);
        }

        public static bool IsSpecRunProject(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecRun") != null ||
                VsxHelper.GetReference(project, "SpecFlow.Plus.Runner") != null;
        }
    }
}