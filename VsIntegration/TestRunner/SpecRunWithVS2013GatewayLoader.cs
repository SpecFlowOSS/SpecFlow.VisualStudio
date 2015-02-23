using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class SpecRunWithVS2013GatewayLoader : AutoTestRunnerGatewayLoader
    {
        public SpecRunWithVS2013GatewayLoader()
            : base(TestRunnerTool.VisualStudio2012)
        {
        }

        public override bool CanUse(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecRun") != null; // would make sense to check for version 1.2 or above, but too complicated
        }
    }
}
