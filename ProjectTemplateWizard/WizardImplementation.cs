using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace ProjectTemplateWizard
{
    public class WizardImplementation : IWizard
    {
        private UserInputForm _inputForm;

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
                // Display a form to the user. The form collects
                // input for the custom message.
                _inputForm = new UserInputForm();
                _inputForm.ShowDialog();

                // Add custom parameters.
                replacementsDictionary.Add("$targetframework$", UserInputForm.TargetFramework);
                replacementsDictionary.Add("$unittestprovider$", UserInputForm.UnitTestProvider);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // This method is only called for item templates,
        // not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }

    class ComboBoxItem
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public override string ToString() => Text;
    }

    public class UserInputForm : Form
    {
        private readonly ComboBox _targetFrameWorkComboBox;
        private readonly ComboBox _unitTestProviderWorkComboBox;
        private readonly Button _createButton;

        public static string TargetFramework { get; private set; }
        public static string UnitTestProvider { get; private set; }

        public UserInputForm()
        {
            Size = new Size(640, 480);

            var dotNetCoreComboBoxItem = new ComboBoxItem { Value = "netcoreapp3.1", Text = ".Net Core 3.1" };
            var fullFrameworkComboBoxItem = new ComboBoxItem { Value = "net472", Text = ".Net Framework 4.7.2" };

            _targetFrameWorkComboBox = new ComboBox
            {
                Location = new Point(10, 25),
                Size = new Size(150, 25),
                DataSource = new[]
                {
                    dotNetCoreComboBoxItem,
                    fullFrameworkComboBoxItem
                },
                SelectedItem = dotNetCoreComboBoxItem
            };
            Controls.Add(_targetFrameWorkComboBox);

            var runnerComboBoxItem = new ComboBoxItem { Value = "runner", Text = "SpecFlow+ Runner"};
            var xUnitComboBoxItem = new ComboBoxItem { Value = "xunit", Text = "xUnit" };
            var nUnitComboBoxItem = new ComboBoxItem { Value = "nunit", Text = "NUnit" };
            var msTestComboBoxItem = new ComboBoxItem { Value = "mstest", Text = "MSTest" };

            _unitTestProviderWorkComboBox = new ComboBox()
            {
                Location = new Point(10, 75),
                Size = new Size(150, 25),
                DataSource = new[]
                {
                    runnerComboBoxItem,
                    xUnitComboBoxItem,
                    nUnitComboBoxItem,
                    msTestComboBoxItem
                },
                SelectedItem = runnerComboBoxItem
            };
            Controls.Add(_unitTestProviderWorkComboBox);

            _createButton = new Button
            {
                Location = new Point(10, 125),
                Size = new Size(50, 25),
                Text = "Create"
            };
            _createButton.Click += CreateButtonClick;
            Controls.Add(_createButton);
        }
        
        private void CreateButtonClick(object sender, EventArgs e)
        {
            TargetFramework = (_targetFrameWorkComboBox.SelectedValue as ComboBoxItem)?.Value;
            UnitTestProvider = (_unitTestProviderWorkComboBox.SelectedValue as ComboBoxItem)?.Value;
            Close();
        }
    }
}
