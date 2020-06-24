using System;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions
{
    public class FeatureSteps
    {
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Feature Feature { get; set; }
        public Version GeneratorVersion { get; set; }
    }
}