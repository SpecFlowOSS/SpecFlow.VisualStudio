using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.scenariotitle")]
    [Name("gherkin.scenariotitle")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinScenarioTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinScenarioTitleClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Title"; 
        }
    }
}