using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.description")]
    [Name("gherkin.description")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    public sealed class GherkinDescriptionClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinDescriptionClassificationFormat()
        {
            this.DisplayName = "Gherkin Feature/Scenario Description";
            this.IsItalic = true;
        }
    }
}