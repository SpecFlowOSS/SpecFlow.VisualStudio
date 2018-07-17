using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public interface ITestRunnerEngine
    {
        bool RunFromEditor(GherkinLanguageService languageService, bool debug, TestRunnerTool? runnerTool = null);
        bool RunFromProjectItem(ProjectItem projectItem, bool debug, TestRunnerTool? runnerTool = null);
        bool RunFromProject(Project project, bool debug, TestRunnerTool? runnerTool = null);
    }
}