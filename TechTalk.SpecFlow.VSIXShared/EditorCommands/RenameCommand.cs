using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.EditorCommands;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.VsIntegration.Implementation.Bindings.Discovery;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands
{
    public class RenameCommand
    {
        private readonly IGherkinLanguageServiceFactory _gherkinLanguageServiceFactory;
        private readonly IProjectScopeFactory _projectScopeFactory;
        private readonly IStepNameReplacer _stepNameReplacer;

        public RenameCommand(IGherkinLanguageServiceFactory gherkinLanguageServiceFactory, IProjectScopeFactory projectScopeFactory, IStepNameReplacer stepNameReplacer)
        {
            _gherkinLanguageServiceFactory = gherkinLanguageServiceFactory;
            _projectScopeFactory = projectScopeFactory;
            this._stepNameReplacer = stepNameReplacer;
        }

        public bool Rename(GherkinEditorContext editorContext)
        {
            var step = GetCurrentStep(editorContext);
            if (step == null)
                return false;

            var stepBinding = GetSingleStepDefinitionBinding(editorContext, step);
            if (stepBinding == null)
                return false;

            var codeFunction = FindBindingMethodCodeFunction(editorContext, stepBinding);
            if (codeFunction == null)
                return false;

            var newStepRegex = PromptForNewStepRegex(stepBinding.Regex);

            if (string.IsNullOrEmpty(newStepRegex))
                return false;

            var stepInstancesToRename = FindAllStepMatchingStepInstances(codeFunction.DTE.ActiveDocument, stepBinding.Method);
            foreach (var stepInstanceToRename in stepInstancesToRename)
            {
                RenameStep(stepInstanceToRename, newStepRegex, stepBinding);
            }

            ReplaceStepBindingAttribute(codeFunction, stepBinding, newStepRegex);

            return true;
        }

        private static string PromptForNewStepRegex(Regex currentRegex)
        {
            var stringRegex = FormatRegexForDisplay(currentRegex);
            var newStepRegex = Interaction.InputBox("Give a new name to the step" + Environment.NewLine + stringRegex, "Rename step", stringRegex);
            if (newStepRegex != stringRegex)
                return newStepRegex;

            return string.Empty;
        }

        private IEnumerable<StepInstanceWithProjectScope> FindAllStepMatchingStepInstances(Document document, IBindingMethod bindingMethod)
        {
            var projectScopes = GetProjectScopes(document).ToArray();
            if (projectScopes.Any(ps => !ps.StepSuggestionProvider.Populated))
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to steps");
                return new StepInstanceWithProjectScope[0];
            }
            return projectScopes.SelectMany(ps => GetMatchingSteps(bindingMethod, ps)).ToArray();
        }

        private void RenameStep(StepInstanceWithProjectScope stepInstance, string newStepRegex, IStepDefinitionBinding binding)
        {
            var featureFileDocument = JumpToStep(stepInstance);
            if (featureFileDocument == null)
                return;

            var stepEditorContext = GherkinEditorContext.FromDocument(featureFileDocument, _gherkinLanguageServiceFactory);

            var stepToRename = GetCurrentStep(stepEditorContext);
            if (stepToRename == null)
                return;

            if (!binding.Regex.IsMatch(stepToRename.Text))
                return;

            var stepLineStart = stepEditorContext.TextView.Selection.Start.Position.GetContainingLine();
            using (var stepNameTextEdit = stepLineStart.Snapshot.TextBuffer.CreateEdit())
            {
                var line = stepLineStart.Snapshot.GetLineFromLineNumber(stepLineStart.LineNumber);
                var lineText = line.GetText();
                var trimmedText = lineText.Trim();
                var numLeadingWhiteSpaces = lineText.Length - trimmedText.Length;

                var actualStepName = trimmedText.Substring(stepToRename.Keyword.Length);
                var newStepName = _stepNameReplacer.BuildStepNameWithNewRegex(actualStepName, newStepRegex, binding);

                var stepNamePosition = line.Start.Position + numLeadingWhiteSpaces +  stepToRename.Keyword.Length;
                stepNameTextEdit.Replace(stepNamePosition, actualStepName.Length, newStepName);

                stepNameTextEdit.Apply();
            }
        }

        private IEnumerable<VsProjectScope> GetProjectScopes(Document activeDocument)
        {
            var projectScopes = _projectScopeFactory.GetProjectScopesFromBindingProject(activeDocument.ProjectItem.ContainingProject);
            return projectScopes.OfType<VsProjectScope>();
        }

        private static IEnumerable<StepInstanceWithProjectScope> GetMatchingSteps(IBindingMethod bindingMethod, VsProjectScope projectScope)
        {
            return projectScope.StepSuggestionProvider.GetMatchingInstances(bindingMethod)
                .Where(si => si is ISourceFilePosition)
                .Distinct(StepInstanceComparer.Instance)
                .OrderBy(si => si, StepInstanceComparer.Instance)
                .Select(si => new StepInstanceWithProjectScope(si, projectScope));
        }

        private static IStepDefinitionBinding GetSingleStepDefinitionBinding(GherkinEditorContext editorContext, GherkinStep step)
        {
            var bindingMatchService = editorContext.LanguageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
                return null;
            }

            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            CultureInfo bindingCulture = editorContext.ProjectScope.SpecFlowConfiguration.BindingCulture ?? step.StepContext.Language;
            var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);

            if (candidatingMatches.Count > 1 || !match.Success)
            {
                MessageBox.Show("Cannot rename automatically. You need to have a single and unique binding for this step.");
                return null;
            }

            return match.StepBinding;
        }

        private static CodeFunction FindBindingMethodCodeFunction(GherkinEditorContext editorContext, IStepDefinitionBinding binding)
        {
            return new VsBindingMethodLocator().FindCodeFunction(((VsProjectScope)editorContext.ProjectScope), binding.Method);
        }

        private void ReplaceStepBindingAttribute(CodeFunction codeFunction, IStepDefinitionBinding binding, string newRegex)
        {
            if (!codeFunction.ProjectItem.IsOpen)
            {
                codeFunction.ProjectItem.Open();
            }
            
            var formattedOldRegex = FormatRegexForDisplay(binding.Regex);

            var navigatePoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartHeader);
            navigatePoint.TryToShow();
            navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);

            var stepBindingEditorContext = GherkinEditorContext.FromDocument(codeFunction.DTE.ActiveDocument, _gherkinLanguageServiceFactory);
            var attributeLinesToUpdate = stepBindingEditorContext.TextView.TextViewLines.Where(x => x.Start.GetContainingLine().GetText().Contains("\"" + formattedOldRegex + "\""));

            foreach (var attributeLineToUpdate in attributeLinesToUpdate)
            {
                using (var textEdit = attributeLineToUpdate.Snapshot.TextBuffer.CreateEdit())
                {
                    var regexStart = attributeLineToUpdate.Start.GetContainingLine().GetText().IndexOf(formattedOldRegex);
                    textEdit.Replace(attributeLineToUpdate.Start.Position + regexStart, formattedOldRegex.Length, newRegex);
                    textEdit.Apply();
                }
            }
        }

        private static GherkinStep GetCurrentStep(GherkinEditorContext editorContext)
        {
            var fileScope = editorContext.LanguageService.GetFileScope(waitForLatest: true);
            if (fileScope == null)
                return null;

            SnapshotPoint caret = editorContext.TextView.Caret.Position.BufferPosition;
            IStepBlock block;
            var step = fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber, out block);

            if (step != null && block is IScenarioOutlineBlock)
                step = step.GetSubstitutedStep((IScenarioOutlineBlock)block);

            return step;
        }

        private static Document JumpToStep(StepInstanceWithProjectScope stepInstance)
        {
            var sourceFilePosition = ((ISourceFilePosition)stepInstance.StepInstance);
            var featureFile = VsxHelper.GetAllPhysicalFileProjectItem(stepInstance.ProjectScope.Project).FirstOrDefault(
                pi => VsxHelper.GetProjectRelativePath(pi).Equals(sourceFilePosition.SourceFile));

            if (featureFile == null)
                return null;

            if (!featureFile.IsOpen)
                featureFile.Open();

            GoToLine(featureFile, sourceFilePosition.FilePosition.Line);
            return featureFile.Document;
        }

        private static void GoToLine(ProjectItem projectItem, int line)
        {
            TextDocument codeBehindTextDocument = (TextDocument)projectItem.Document.Object("TextDocument");

            var navigatePoint = codeBehindTextDocument.StartPoint.CreateEditPoint();
            navigatePoint.MoveToLineAndOffset(line, 1);
            navigatePoint.TryToShow();
            navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
        }

        private static string FormatRegexForDisplay(Regex regex)
        {
            return RemoveDoubleQuotes(TrimLast(TrimFirst(regex.ToString())));
        }

        private static string RemoveDoubleQuotes(string value)
        {
            return value.Replace("\"\"", "\""); ;
        }

        private static string TrimFirst(string value)
        {
            return value.Remove(0, 1);
        }

        private static string TrimLast(string value)
        {
            return value.Remove(value.Length - 1, 1);
        }
    }
}
