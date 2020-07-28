using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using BoDi;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Implementation;

namespace ProjectTemplateWizard
{
    public class WizardImplementation : IWizard
    {
        private UserInputDialog _inputDialog;
        private string _projectDirectory;
        private string _solutionDirectory;
        private Project _project;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;

        public WizardImplementation()
        {
            var defaultDependencyProvider = new DefaultDependencyProvider();
            var container = new ObjectContainer();
            defaultDependencyProvider.RegisterDependencies(container);
            container.RegisterTypeAs<MockIntegrationOptionsProvider, IIntegrationOptionsProvider>();
            container.RegisterTypeAs<MockIServiceProvider, IServiceProvider>();
            try
            {
                _analyticsTransmitter = container.Resolve<IAnalyticsTransmitter>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        // This method is called before opening any item that
        // has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            // We need the project in method RunFinished below.
            _project = project;
        }

        // This method is only called for item templates,
        // not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
            // A workaround so that the Visual extension can recognize the bindings:
            // Trigger parsing of step definition file(s) by saving them.
            // Use a delay of 10 seconds on a different thread before saving the C# source files
            // to allow some time for initializing the project.
            // Otherwise, the bindings might not get recognized by the SpecFlow Visual Studio extension
            // until saving the C# source files manually.
            new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(10000);
                try
                {
                    foreach (var cSharpCodeFile in CollectCSharpCodeFileProjectItems(_project.ProjectItems))
                    {
                        cSharpCodeFile.Open();
                        cSharpCodeFile.Save();
                    }
                }
                catch (Exception)
                {
                    // NOOP
                }
            }).Start();
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
                _projectDirectory = replacementsDictionary["$destinationdirectory$"];
                _solutionDirectory = replacementsDictionary["$solutiondirectory$"];

                _inputDialog = new UserInputDialog();
                bool? dialogResult = _inputDialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    // Add custom parameters.
                    replacementsDictionary.Add("$dotnetframework$", _inputDialog.DotNetFramework);
                    replacementsDictionary.Add("$unittestframework$", _inputDialog.UnitTestFramework);

                    // Add analytics.
                    _analyticsTransmitter.TransmitProjectTemplateUsage(_inputDialog.DotNetFramework, _inputDialog.UnitTestFramework);
                }
                else
                {
                    CancelProjectCreation();
                }
            }
            catch (WizardBackoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                CancelProjectCreation();
            }
        }

        // This method is only called for item templates,
        // not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void CancelProjectCreation()
        {
            Cleanup();

            // Cancel the project creation.
            throw new WizardBackoutException();
        }

        private void Cleanup()
        {
            if (Directory.Exists(_projectDirectory))
            {
                Directory.Delete(_projectDirectory, true);
            }

            if (_projectDirectory != _solutionDirectory &&
                Directory.Exists(_solutionDirectory) &&
                !Directory.EnumerateFileSystemEntries(_solutionDirectory).Any())
            {
                Directory.Delete(_solutionDirectory);
            }
        }

        private static List<ProjectItem> CollectCSharpCodeFileProjectItems(ProjectItems projectItems)
        {
            var collectedCSharpCodeFileProjectItems = new List<ProjectItem>();

            if (projectItems == null)
            {
                return collectedCSharpCodeFileProjectItems;
            }

            foreach (ProjectItem projectItem in projectItems)
            {
                // Include .cs files but exclude generated feature code behind files.
                if (projectItem.Name.EndsWith(".cs"))
                {
                    collectedCSharpCodeFileProjectItems.Add(projectItem);
                }
                
                collectedCSharpCodeFileProjectItems.AddRange(CollectCSharpCodeFileProjectItems(projectItem.ProjectItems));
            }

            return collectedCSharpCodeFileProjectItems;
        }
    }
}
