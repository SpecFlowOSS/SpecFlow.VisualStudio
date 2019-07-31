using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Implementation.Options;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Options
{
    [Export(typeof(IIntegrationOptionsProvider))]
    public class IntegrationOptionsProvider : IIntegrationOptionsProvider
    {
        internal static IntegrationOptions cachedOptions = null;

        public const string SPECFLOW_OPTIONS_CATEGORY = "SpecFlow";
        public const string SPECFLOW_GENERAL_OPTIONS_PAGE = "General";

        private DTE dte;

        public IntegrationOptionsProvider()
        {
        }

        public IntegrationOptionsProvider(DTE dte)
        {
            this.dte = dte;
        }

        private static T GetGeneralOption<T>(DTE dte, string optionName, T defaultValue = default(T))
        {
            return VsxHelper.GetOption(dte, SPECFLOW_OPTIONS_CATEGORY, SPECFLOW_GENERAL_OPTIONS_PAGE, optionName, defaultValue);
        }

        private static IntegrationOptions GetOptions(DTE dte)
        {
            var options = cachedOptions;
            if (options != null)
                return options;

            int maxStepInstancesSuggestions;
            options = new IntegrationOptions
            {
                EnableSyntaxColoring = GetGeneralOption(dte, "EnableSyntaxColoring", OptionDefaultValues.EnableSyntaxColoringDefaultValue),
                EnableOutlining = GetGeneralOption(dte, "EnableOutlining", OptionDefaultValues.EnableOutliningDefaultValue),
                EnableIntelliSense = GetGeneralOption(dte, "EnableIntelliSense", OptionDefaultValues.EnableIntelliSenseDefaultValue),
                LimitStepInstancesSuggestions = int.TryParse(GetGeneralOption(dte, "MaxStepInstancesSuggestions", OptionDefaultValues.MaxStepInstancesSuggestionsDefaultValue), out maxStepInstancesSuggestions),
                MaxStepInstancesSuggestions = maxStepInstancesSuggestions,
                EnableAnalysis = GetGeneralOption(dte, "EnableAnalysis", OptionDefaultValues.EnableAnalysisDefaultValue),
                EnableTableAutoFormat = GetGeneralOption(dte, "EnableTableAutoFormat", OptionDefaultValues.EnableTableAutoFormatDefaultValue),
                EnableStepMatchColoring = GetGeneralOption(dte, "EnableStepMatchColoring", OptionDefaultValues.EnableStepMatchColoringDefaultValue),
                EnableTracing = GetGeneralOption(dte, "EnableTracing", OptionDefaultValues.EnableTracingDefaultValue),
                TracingCategories = GetGeneralOption(dte, "TracingCategories", OptionDefaultValues.TracingCategoriesDefaultValue),
                DisableRegenerateFeatureFilePopupOnConfigChange = GetGeneralOption(dte, "DisableRegenerateFeatureFilePopupOnConfigChange", OptionDefaultValues.DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue),
                GenerationMode = GetGeneralOption(dte, "GenerationMode", OptionDefaultValues.GenerationModeDefaultValue),
                CodeBehindFileGeneratorPath = GetGeneralOption(dte, "PathToCodeBehindGeneratorExe", OptionDefaultValues.CodeBehindFileGeneratorPath),
                CodeBehindFileGeneratorExchangePath = GetGeneralOption(dte, "CodeBehindFileGeneratorExchangePath", OptionDefaultValues.CodeBehindFileGeneratorExchangePath),
                OptOutDataCollection = GetGeneralOption(dte, "OptOutDataCollection", OptionDefaultValues.DefaultOptOutDataCollection),
                NormalizeLineBreaks = GetGeneralOption(dte, "NormalizeLineBreaks", OptionDefaultValues.NormalizeLineBreaksDefaultValue),
                LineBreaksBeforeScenario = GetGeneralOption(dte, "LineBreaksBeforeScenario", OptionDefaultValues.DefaultLineBreaksBeforeScenario),
                LineBreaksBeforeExamples = GetGeneralOption(dte, "LineBreaksBeforeExamples", OptionDefaultValues.DefaultLineBreaksBeforeExamples),
                UseTabsForIndent = GetGeneralOption(dte, "UseTabsForIndent", OptionDefaultValues.UseTabsForIndentDefaultValue),
                FeatureIndent = GetGeneralOption(dte, "FeatureIndent", OptionDefaultValues.DefaultFeatureIndent),
                ScenarioIndent = GetGeneralOption(dte, "ScenarioIndent", OptionDefaultValues.DefaultScenarioIndent),
                StepIndent = GetGeneralOption(dte, "StepIndent", OptionDefaultValues.DefaultStepIndent),
                TableIndent = GetGeneralOption(dte, "TableIndent", OptionDefaultValues.DefaultTableIndent),
                MultilineIndent = GetGeneralOption(dte, "MultilineIndent", OptionDefaultValues.DefaultMultilineIndent),
                ExampleIndent = GetGeneralOption(dte, "ExampleIndent", OptionDefaultValues.DefaultExampleIndent),
                LegacyEnableSpecFlowSingleFileGeneratorCustomTool = GetGeneralOption(dte, "LegacyEnableSpecFlowSingleFileGeneratorCustomTool", OptionDefaultValues.LegacyEnableSpecFlowSingleFileGeneratorCustomTool)
            };
            cachedOptions = options;
            return options;
        }

        [Import]
        internal SVsServiceProvider ServiceProvider
        {
            set { dte = VsxHelper.GetDte(value); }
        }

        public IntegrationOptions GetOptions()
        {
            return GetOptions(dte);
        }

        public void ClearCache()
        {
            cachedOptions = null;
        }
    }
}