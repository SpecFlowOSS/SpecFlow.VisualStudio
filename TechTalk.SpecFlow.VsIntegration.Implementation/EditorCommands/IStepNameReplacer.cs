using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands
{
    public interface IStepNameReplacer
    {
        public string BuildStepNameWithNewRegex(string stepName, string newStepRegex, IStepDefinitionBinding binding);
    }
}
