using TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Commands
{
    public interface IGoToStepDefinitionCommand : IEditorCommand
    {
        bool CanGoToDefinition(GherkinEditorContext editorContext);
        bool GoToDefinition(GherkinEditorContext editorContext);
    }
}