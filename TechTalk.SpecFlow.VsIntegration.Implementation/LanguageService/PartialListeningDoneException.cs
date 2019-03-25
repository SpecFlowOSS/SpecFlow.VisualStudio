using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class PartialListeningDoneException : ScanningCancelledException
    {
        public IScenarioBlock FirstUnchangedScenario { get; private set; }

        public PartialListeningDoneException(IScenarioBlock firstUnchangedScenario)
        {
            FirstUnchangedScenario = firstUnchangedScenario;
        }
    }
}