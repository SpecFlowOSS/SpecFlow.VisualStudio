using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.SingleFileGenerator;

namespace TechTalk.SpecFlow.VsIntegration.Options
{
    /// <summary>
    // Extends a standard dialog functionality for implementing ToolsOptions pages, 
    // with support for the Visual Studio automation model, Windows Forms, and state 
    // persistence through the Visual Studio settings mechanism.
    /// </summary>
    [Guid("D41B81C9-8501-4124-B75F-0F194E37178C")]
    [ComVisible(true)]
    public class OptionsPageGeneral : DialogPage
    {
        [Category("Analysis Settings")]
        [Description("Controls whether SpecFlow should collect binding information and step suggestions from the feature files. (restart required)")]
        [DisplayName(@"Enable project-wide analysis")]
        [DefaultValue(IntegrationOptionsProvider.EnableAnalysisDefaultValue)]
        public bool EnableAnalysis { get; set; }

        [Category("Analysis Settings")]
        [Description("Controls whether SpecFlow Visual Studio integration should offer re-generating the feature files on configuration change.")]
        [DisplayName(@"Disable re-generate feature file popup")]
        [DefaultValue(IntegrationOptionsProvider.DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue)]
        public bool DisableRegenerateFeatureFilePopupOnConfigChange { get; set; }

        private bool enableSyntaxColoring = true;
        private CustomToolSwitch _customToolSwitch;

        [Category("Editor Settings")]
        [Description("Controls whether the different syntax elements of the feature files should be indicated in the editor.")]
        [DisplayName(@"Enable Syntax Coloring")]
        [DefaultValue(IntegrationOptionsProvider.EnableSyntaxColoringDefaultValue)]
        [RefreshProperties(RefreshProperties.All)]
        public bool EnableSyntaxColoring
        {
            get { return enableSyntaxColoring; }
            set
            {
                enableSyntaxColoring = value;
                if (!value)
                {
                    EnableOutlining = false;
                    EnableIntelliSense = false;
                }
            }
        }

        [Category("Editor Settings")]
        [Description("Controls whether the scenario blocks of the feature files should be outlined in the editor.")]
        [DisplayName(@"Enable Outlining")]
        [DefaultValue(IntegrationOptionsProvider.EnableOutliningDefaultValue)]
        public bool EnableOutlining { get; set; }

        [Category("Editor Settings")]
        [Description("Controls whether the step definition match status should be indicated with a different color in the editor. (beta)")]
        [DisplayName(@"Enable Step Match Coloring")]
        [DefaultValue(IntegrationOptionsProvider.EnableStepMatchColoringDefaultValue)]
        public bool EnableStepMatchColoring { get; set; }

        [Category("Editor Settings")]
        [Description("Controls whether the tables should be formatted automatically when you type \"|\" character.")]
        [DisplayName(@"Enable Table Formatting")]
        [DefaultValue(IntegrationOptionsProvider.EnableTableAutoFormatDefaultValue)]
        public bool EnableTableAutoFormat { get; set; }


        [Category("IntelliSense")]
        [Description("Controls whether completion lists should be displayed for the feature files.")]
        [DisplayName(@"Enable IntelliSense")]
        [DefaultValue(IntegrationOptionsProvider.EnableIntelliSenseDefaultValue)]
        public bool EnableIntelliSense { get; set; }

        private string _maxStepInstancesSuggestions = String.Empty;
        [Category("IntelliSense")]
        [Description("Limit quantity of IntelliSense step instances suggestions for each step template.")]
        [DisplayName(@"Max Step Instances Suggestions")]
        [DefaultValue(IntegrationOptionsProvider.MaxStepInstancesSuggestionsDefaultValue)]
        public string MaxStepInstancesSuggestions {
            get { return _maxStepInstancesSuggestions; }
            set
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue) && parsedValue >= 0)
                {
                    _maxStepInstancesSuggestions = parsedValue.ToString();
                }
                else
                {
                    _maxStepInstancesSuggestions = string.Empty;
                }
            }
        }

        [Category("Tracing")]
        [Description("Controls whether diagnostic trace messages should be emitted to the output window.")]
        [DisplayName(@"Enable Tracing")]
        [DefaultValue(IntegrationOptionsProvider.EnableTracingDefaultValue)]
        public bool EnableTracing { get; set; }

        [Category("Tracing")]
        [Description("Specifies the enabled the tracing categories in a comma-seperated list. Use \"all\" to trace all categories.")]
        [DisplayName(@"Tracing Categories")]
        [DefaultValue(IntegrationOptionsProvider.TracingCategoriesDefaultValue)]
        public string TracingCategories { get; set; }

        [Category("Test Execution")]
        [Description("Specifies the test runner tool to be used to execute the SpecFlow scenarios.")]
        [DisplayName(@"Test Runner Tool")]
        [DefaultValue(IntegrationOptionsProvider.TestRunnerToolDefaultValue)]
        public TestRunnerTool TestRunnerTool { get; set; }

        [Category("Code Behind File Generation")]
        [Description("Specifies the mode how the code behind file is generated")]
        [DisplayName("Generation Mode")]
        [DefaultValue(IntegrationOptionsProvider.GenerationModeDefaultValue)]
        public GenerationMode GenerationMode { get; set; }

        [Category("Code Behind File Generation")]
        [Description("Specifies the path to TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe")]
        [DisplayName("Path to cmd tool")]
        [DefaultValue(IntegrationOptionsProvider.CodeBehindFileGeneratorPath)]
        public string PathToCodeBehindGeneratorExe { get; set; }

        [Category("Code Behind File Generation")]
        [Description("Specifies the path where to save data exchange files (Default: %TEMP%)")]
        [DisplayName("Data Exchange Path")]
        [DefaultValue(IntegrationOptionsProvider.CodeBehindFileGeneratorPath)]
        public string CodeBehindFileGeneratorExchangePath { get; set; }


        [Category("Legacy")]
        [Description("Enables")]
        [DisplayName("Enable SpecFlowSingleFileGenerator CustomTool")]
        [DefaultValue(false)]
        public bool LegacyEnableSpecFlowSingleFileGeneratorCustomTool { get; set; }
        public const string UsageStatisticsCategory = "Usage statistics";

        [Category(UsageStatisticsCategory)]
        [Description("Disables sending error reports and other statistics transmissions.")]
        [DisplayName("Opt-Out of Data Collection")]
        [DefaultValue(IntegrationOptionsProvider.DefaultOptOutDataCollection)]
        public bool OptOutDataCollection { get; set; }

        public OptionsPageGeneral()
        {
            _customToolSwitch = new CustomToolSwitch(Dte);

            EnableAnalysis = IntegrationOptionsProvider.EnableAnalysisDefaultValue;
            EnableSyntaxColoring = IntegrationOptionsProvider.EnableSyntaxColoringDefaultValue;
            EnableOutlining = IntegrationOptionsProvider.EnableOutliningDefaultValue;
            EnableIntelliSense = IntegrationOptionsProvider.EnableIntelliSenseDefaultValue;
            MaxStepInstancesSuggestions = IntegrationOptionsProvider.MaxStepInstancesSuggestionsDefaultValue;
            EnableTableAutoFormat = IntegrationOptionsProvider.EnableTableAutoFormatDefaultValue;
            EnableStepMatchColoring = IntegrationOptionsProvider.EnableStepMatchColoringDefaultValue;
            EnableTracing = IntegrationOptionsProvider.EnableTracingDefaultValue;
            TracingCategories = IntegrationOptionsProvider.TracingCategoriesDefaultValue;
            TestRunnerTool = IntegrationOptionsProvider.TestRunnerToolDefaultValue;
            DisableRegenerateFeatureFilePopupOnConfigChange = IntegrationOptionsProvider.DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue;
            GenerationMode = IntegrationOptionsProvider.GenerationModeDefaultValue;
            PathToCodeBehindGeneratorExe = IntegrationOptionsProvider.CodeBehindFileGeneratorPath;
            CodeBehindFileGeneratorExchangePath = IntegrationOptionsProvider.CodeBehindFileGeneratorExchangePath;
            OptOutDataCollection = IntegrationOptionsProvider.DefaultOptOutDataCollection;
            LegacyEnableSpecFlowSingleFileGeneratorCustomTool = _customToolSwitch.IsEnabled();
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            LegacyEnableSpecFlowSingleFileGeneratorCustomTool = _customToolSwitch.IsEnabled();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            IntegrationOptionsProvider.cachedOptions = null;

            if (LegacyEnableSpecFlowSingleFileGeneratorCustomTool)
            {
                _customToolSwitch.Enable();
            }
            else
            {
                _customToolSwitch.Disable();
            }
        }

        private DTE Dte
        {
            get { return Package.GetGlobalService(typeof(DTE)) as DTE; }
        }
    }
}
