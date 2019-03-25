using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.unboundsteptext")]
    [Name("gherkin.unboundsteptext")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinUnboundStepTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinUnboundStepTextClassificationFormat()
        {
            this.DisplayName = "Gherkin Unbound Step Text";
            this.ForegroundColor = Color.FromRgb(105, 54, 153);
        }
    }
}