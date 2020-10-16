using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace ProjectTemplateWizard
{
    public class UserInputDialog : DialogWindow
    {
        readonly UserInputControl _userInputControl;
        
        public string DotNetFramework { get; private set; }
        public string UnitTestFramework { get; private set; }
        public bool FluentAssertionsIncluded { get; private set; }

        private static SolidColorBrush ToBrush(ThemeResourceKey key)
        {
            var color = VSColorTheme.GetThemedColor(key);
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        internal UserInputDialog()
        {
            _userInputControl = new UserInputControl();

            MinWidth = _userInputControl.MinWidth;
            MinHeight = _userInputControl.MinHeight;
            Width = _userInputControl.Width;
            Height = _userInputControl.Height;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Content = _userInputControl.Content;
            _userInputControl.CloseButton.Click += CloseDialog;
            _userInputControl.BackButton.Click += CloseDialog;
            _userInputControl.CreateButton.Click += SetCustomParametersAndCloseDialog;

            WindowStyle = WindowStyle.None;
            WindowChrome.SetWindowChrome(this, new WindowChrome
            {
                CaptionHeight = 32,
                ResizeBorderThickness = SystemParameters.WindowResizeBorderThickness
            });
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
            FluentAssertionsIncluded = _userInputControl.FluentAssertionsCheckBox.IsChecked.GetValueOrDefault();

            DialogResult = true;
            Close();
        }
    }
}
