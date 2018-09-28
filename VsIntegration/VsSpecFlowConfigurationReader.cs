using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration
{
    internal class VsSpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly Project project;

        public VsSpecFlowConfigurationReader(Project project, IIdeTracer tracer) : base(tracer)
        {
            this.project = project;
        }

        protected override string GetConfigFileContent()
        {
            var projectItem = VsxHelper.FindProjectItemByProjectRelativePath(project, "specflow.json") ?? 
                              VsxHelper.FindProjectItemByProjectRelativePath(project, "app.config");
            if (projectItem == null)
                return null;

            return VsxHelper.GetFileContent(projectItem, true);
        }
    }
}