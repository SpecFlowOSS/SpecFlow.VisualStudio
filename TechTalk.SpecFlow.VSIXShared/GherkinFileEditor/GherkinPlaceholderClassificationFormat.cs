using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.placeholder")]
    [Name("gherkin.placeholder")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinPlaceholderClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinPlaceholderClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Outline Placeholder"; 
        }
    }
}