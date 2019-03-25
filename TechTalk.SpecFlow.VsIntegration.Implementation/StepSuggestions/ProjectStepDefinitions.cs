using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public class ProjectStepDefinitions
    {
        public string ProjectName { get; set; }
        public List<FileStepDefinitions> FileStepDefinitions { get; set; }
    }
}
