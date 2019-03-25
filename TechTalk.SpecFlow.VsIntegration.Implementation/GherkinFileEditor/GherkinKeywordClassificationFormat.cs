using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.VsIntegration.GherkinFileEditor
{
    // exports a classification format for the classification type gherkin.tag
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.keyword")]
    [Name("gherkin.keyword")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinKeywordClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinKeywordClassificationFormat()
        {
            this.DisplayName = "Gherkin Keyword"; 
        }
    }

    // exports a classification format for the classification type gherkin.tag

    // exports a classification format for the classification type gherkin.tag

    // exports a classification format for the classification type gherkin.multilinetext

    // exports a classification format for the classification type gherkin.tablecell

    // exports a classification format for the classification type gherkin.tableheader

    // exports a classification format for the classification type gherkin.description

    // exports a classification format for the classification type gherkin.placeholder

    // exports a classification format for the classification type gherkin.placeholder

    // exports a classification format for the classification type gherkin.steptext

    // exports a classification format for the classification type gherkin.unboundsteptext

    // exports a classification format for the classification type gherkin.stepargument

    // exports a classification format for the classification type gherkin.placeholder
}
