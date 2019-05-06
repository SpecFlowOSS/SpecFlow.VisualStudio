using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Bindings.Discovery;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions;
using TechTalk.SpecFlow.VsIntegration.Implementation.UI;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;
using TextSelection = EnvDTE.TextSelection;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Commands
{
    public class GoToStepsCommand : IGoToStepsCommand
    {
        private readonly DTE dte;
        private readonly IProjectScopeFactory projectScopeFactory;
        private readonly IIdeTracer tracer;

        public GoToStepsCommand(DTE dte, IProjectScopeFactory projectScopeFactory, IIdeTracer tracer)
        {
            this.dte = dte;
            this.projectScopeFactory = projectScopeFactory;
            this.tracer = tracer;
        }

        public bool IsEnabled(Document activeDocument)
        {
            var bindingMethod = GetSelectedBindingMethod(activeDocument);
            return bindingMethod != null && IsStepDefinition(bindingMethod, activeDocument);
        }

        public void Invoke(Document activeDocument)
        {
            var bindingMethod = GetSelectedBindingMethod(activeDocument);

            var projectScopes = GetProjectScopes(activeDocument).ToArray();
            if (projectScopes.Any(ps => !ps.StepSuggestionProvider.Populated))
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to steps");
                return;
            }
            var candidatingPositions = projectScopes.SelectMany(ps => GetMatchingSteps(bindingMethod, ps)).ToArray();

            if (candidatingPositions.Length == 0)
            {
                MessageBox.Show("No matching step found.", "Go to steps");
                return;
            }

            if (candidatingPositions.Length == 1)
            {
                JumpToStep(candidatingPositions[0]);
                return;
            }

            var menuBuilder = new ContextMenuBuilder();
            menuBuilder.Title = "Go to steps: Multiple matching steps found.";
            menuBuilder.AddRange(candidatingPositions.Select((si, index) =>
                                                             new ContextMenuBuilder.ContextCommandItem(
                                                                 GetStepInstanceGotoTitle(si),
                                                                 () => JumpToStep(si))));

            VsContextMenuManager.ShowContextMenu(menuBuilder.ToContextMenu(), dte);
        }

        private string GetStepInstanceGotoTitle(StepInstanceWithProjectScope stepInstanceWithProjectScope)
        {
            var stepInstance = stepInstanceWithProjectScope.StepInstance;
            return stepInstance.GetFileScopedLabel();
        }

        private void JumpToStep(StepInstanceWithProjectScope stepInstanceWithProjectScope)
        {
            var position = ((ISourceFilePosition)stepInstanceWithProjectScope.StepInstance);
            var featureProjItem = VsxHelper.GetAllPhysicalFileProjectItem(stepInstanceWithProjectScope.ProjectScope.Project).FirstOrDefault(
                pi => VsxHelper.GetProjectRelativePath(pi).Equals(position.SourceFile));

            if (featureProjItem == null)
                return;

            if (!featureProjItem.IsOpen)
                featureProjItem.Open();
            GoToLine(featureProjItem, position.FilePosition.Line);
        }

        private static void GoToLine(ProjectItem projectItem, int line)
        {
            TextDocument codeBehindTextDocument = (TextDocument) projectItem.Document.Object("TextDocument");

            EditPoint navigatePoint = codeBehindTextDocument.StartPoint.CreateEditPoint();
            navigatePoint.MoveToLineAndOffset(line, 1);
            navigatePoint.TryToShow();
            navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
        }
        
        private IEnumerable<VsProjectScope> GetProjectScopes(Document activeDocument)
        {
            var projectScopes = projectScopeFactory.GetProjectScopesFromBindingProject(activeDocument.ProjectItem.ContainingProject);
            return projectScopes.OfType<VsProjectScope>();
        }

        private IEnumerable<StepInstanceWithProjectScope> GetMatchingSteps(IBindingMethod bindingMethod, VsProjectScope projectScope)
        {
            return projectScope.StepSuggestionProvider.GetMatchingInstances(bindingMethod)
                .Where(si => si is ISourceFilePosition)
                .Distinct(StepInstanceComparer.Instance)
                .OrderBy(si => si, StepInstanceComparer.Instance)
                .Select(si => new StepInstanceWithProjectScope(si, projectScope));
        }

        private bool IsStepDefinition(IBindingMethod bindingMethod, Document activeDocument)
        {
            var vsBindingRegistryBuilder = new VsBindingRegistryBuilder(tracer);
            var stepDefinitionBinding = vsBindingRegistryBuilder.GetBindingsFromProjectItem(activeDocument.ProjectItem).FirstOrDefault(sdb => sdb.Method.MethodEquals(bindingMethod));
            return stepDefinitionBinding != null;
        }

        private IBindingMethod GetSelectedBindingMethod(Document activeDocument)
        {
            var codeFunction = GetSelectedFunction(activeDocument);
            if (codeFunction == null)
                return null;
                        
            var bindingReflectionFactory = new VsBindingReflectionFactory();
            return bindingReflectionFactory.CreateBindingMethod(codeFunction);
        }

        private CodeFunction GetSelectedFunction(Document activeDocument)
        {
            var textSelection = (TextSelection)activeDocument.Selection;
            if (textSelection == null)
                return null;
            var codeModel = activeDocument.ProjectItem.FileCodeModel;
            if (codeModel == null)
                return null;

            return VsxHelper.TryGetCodeElementFromActivePoint(codeModel, textSelection) as CodeFunction;
        }
    }
}
