using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.stepargument")]
    [Name("gherkin.stepargument")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinStepArgumentClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinStepArgumentClassificationFormat()
        {
            this.DisplayName = "Gherkin Step Argument";
            this.ForegroundColor = Color.FromRgb(100, 100, 100);
            this.IsItalic = true;
        }
    }
}