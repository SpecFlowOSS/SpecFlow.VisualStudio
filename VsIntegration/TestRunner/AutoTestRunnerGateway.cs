using System;
using System.Linq;
using System.Windows;
using BoDi;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Utils;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public class AutoTestRunnerGateway : ITestRunnerGateway
    {
        private readonly IObjectContainer container;
        private readonly IdeIntegration.Install.IdeIntegration ideIntegration;

        public AutoTestRunnerGateway(IObjectContainer container, InstallServices installServices)
        {
            this.container = container;
            ideIntegration = installServices.IdeIntegration;
        }

        protected virtual IEnumerable<AutoTestRunnerGatewayLoader> GetLoaders()
        {
            yield return new SpecRunWithVS2013GatewayLoader();
            yield return new SpecRunGatewayLoader();
            yield return new ReSharper6GatewayLoader();
            yield return new VisualStudio2013GatewayLoader();
        }

        private ITestRunnerGateway GetCurrentTestRunnerGateway(Project project)
        {
            foreach (var loader in GetLoaders())
            {
                if (loader.CanUse(project))
                    return loader.CreateTestRunner(container);
            }

            MessageBox.Show(
                "Could not find matching test runner. Please specify the test runner tool in 'Tools / Options / SpecFlow'",
                "SpecFlow", MessageBoxButton.OK, MessageBoxImage.Error);

            return null;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, ScenarioOutlineExamplesRow examplesRow, IGherkinFileScope fileScope, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(projectItem.ContainingProject);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunScenario(projectItem, currentScenario, examplesRow, fileScope, debug);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(projectItem.ContainingProject);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(projectItem, debug);
        }

        public bool RunFeatures(Project project, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(project);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(project, debug);
        }
    }
}