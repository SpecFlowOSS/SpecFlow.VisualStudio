using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public class FileStepDefinitions
    {
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<StepDefinitionBindingItem> StepDefinitions { get; set; }
    }
}