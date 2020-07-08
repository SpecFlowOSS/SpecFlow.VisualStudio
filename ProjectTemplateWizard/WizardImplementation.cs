using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;

namespace ProjectTemplateWizard
{
    public class WizardImplementation : IWizard
    {
        private UserInputDialog _inputDialog;
        private string _projectDirectory;
        private string _solutionDirectory;

        // This method is called before opening any item that
        // has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
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
    }

    public class UserInputDialog : DialogWindow
    {
        readonly UserInputControl _userInputControl;
        
        public string DotNetFramework { get; private set; }
        public string UnitTestFramework { get; private set; }

        private static SolidColorBrush ToBrush(ThemeResourceKey key)
        {
            var color = VSColorTheme.GetThemedColor(key);
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        internal UserInputDialog()
        {
            IsCloseButtonEnabled = true;

            _userInputControl = new UserInputControl();

            MinWidth = _userInputControl.MinWidth;
            MinHeight = _userInputControl.MinHeight;
            Width = _userInputControl.Width;
            Height = _userInputControl.Height;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Content = _userInputControl.Content;
            _userInputControl.BackButton.Click += CloseDialog;
            _userInputControl.CreateButton.Click += SetCustomParametersAndCloseDialog;

            Background = ToBrush(EnvironmentColors.ToolboxBackgroundColorKey);
        }

        private void CloseDialog(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetCustomParametersAndCloseDialog(object sender, EventArgs e)
        {
            DotNetFramework = _userInputControl.DotNetFrameworkComboBox.SelectedValue as string;
            UnitTestFramework = _userInputControl.UnitTestFrameworkComboBox.SelectedValue as string;

            DialogResult = true;
            Close();
        }
    }
}
