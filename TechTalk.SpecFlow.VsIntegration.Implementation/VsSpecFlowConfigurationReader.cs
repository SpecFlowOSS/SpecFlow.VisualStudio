using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public class VsSpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly Project _project;

        public VsSpecFlowConfigurationReader(Project project, IIdeTracer tracer) : base(tracer)
        {
            _project = project;
        }

        protected override string GetConfigFileContent()
        {
            var projectItem = VsxHelper.FindProjectItemByProjectRelativePath(_project, "specflow.json") ?? 
                              VsxHelper.FindProjectItemByProjectRelativePath(_project, "app.config");
            if (projectItem == null)
            {
                return null;
            }

            return VsxHelper.GetFileContent(projectItem, true);
        }
    }
}
