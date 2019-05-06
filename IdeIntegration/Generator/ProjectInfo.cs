using System;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class ProjectInfo
    {
        public ProjectInfo(string projectName, Version referencedSpecFlowVersion)
        {
            ReferencedSpecFlowVersion = referencedSpecFlowVersion;
            ProjectName = projectName;
        }

        public string ProjectName { get; }

        public Version ReferencedSpecFlowVersion { get; }
    }
}
