using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tablecell")]
    [Name("gherkin.tablecell")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinTableCellClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableCellClassificationFormat()
        {
            this.DisplayName = "Gherkin Table Cell"; 
        }
    }
}