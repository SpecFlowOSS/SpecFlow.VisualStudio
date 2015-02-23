using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class SpecRunWithVsTestExplorerGatewayLoader : SpecRunGatewayLoader
    {
        public SpecRunWithVsTestExplorerGatewayLoader() : base(TestRunnerTool.VisualStudio2012)
        {
        }
    }
}
