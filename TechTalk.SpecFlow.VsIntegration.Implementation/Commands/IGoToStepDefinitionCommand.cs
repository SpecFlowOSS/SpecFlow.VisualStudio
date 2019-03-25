using TechTalk.SpecFlow.VsIntegration.EditorCommands;

namespace TechTalk.SpecFlow.VsIntegration.Commands
{
    public interface IGoToStepDefinitionCommand : IEditorCommand
    {
        bool CanGoToDefinition(GherkinEditorContext editorContext);
        bool GoToDefinition(GherkinEditorContext editorContext);
    }
}