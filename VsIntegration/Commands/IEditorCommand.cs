using EnvDTE;

namespace TechTalk.SpecFlow.VsIntegration.Commands
{
    public interface IEditorCommand
    {
        bool IsEnabled(Document activeDocument);
        void Invoke(Document activeDocument);
    }
}