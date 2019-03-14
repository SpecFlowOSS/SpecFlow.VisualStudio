using System;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Options
{
    [Export(typeof(IIntegrationOptionsProvider))]
    internal class IntegrationOptionsProvider : IIntegrationOptionsProvider
    {
        internal static IntegrationOptions cachedOptions = null;

        public const string SPECFLOW_OPTIONS_CATEGORY = "SpecFlow";
        public const string SPECFLOW_GENERAL_OPTIONS_PAGE = "General";

        public const bool EnableSyntaxColoringDefaultValue = true;
        public const bool EnableOutliningDefaultValue = true;
        public const bool EnableIntelliSenseDefaultValue = true;
        public const string MaxStepInstancesSuggestionsDefaultValue = "";
        public const bool EnableAnalysisDefaultValue = true;
        public const bool EnableTableAutoFormatDefaultValue = true;
        public const bool EnableStepMatchColoringDefaultValue = true;
        public const bool EnableTracingDefaultValue = false;
        public const string TracingCategoriesDefaultValue = "all";
        public const bool DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue = false;
        public const GenerationMode GenerationModeDefaultValue = GenerationMode.OutOfProcess;
        public const string CodeBehindFileGeneratorPath = null;
        public const string CodeBehindFileGeneratorExchangePath = null;
        public const bool DefaultOptOutDataCollection = false;


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
                EnableSyntaxColoring = GetGeneralOption(dte, "EnableSyntaxColoring", EnableSyntaxColoringDefaultValue),
                EnableOutlining = GetGeneralOption(dte, "EnableOutlining", EnableOutliningDefaultValue),
                EnableIntelliSense = GetGeneralOption(dte, "EnableIntelliSense", EnableIntelliSenseDefaultValue),
                LimitStepInstancesSuggestions = int.TryParse(GetGeneralOption(dte, "MaxStepInstancesSuggestions", MaxStepInstancesSuggestionsDefaultValue), out maxStepInstancesSuggestions),
                MaxStepInstancesSuggestions = maxStepInstancesSuggestions,
                EnableAnalysis = GetGeneralOption(dte, "EnableAnalysis", EnableAnalysisDefaultValue),
                EnableTableAutoFormat = GetGeneralOption(dte, "EnableTableAutoFormat", EnableTableAutoFormatDefaultValue),
                EnableStepMatchColoring = GetGeneralOption(dte, "EnableStepMatchColoring", EnableStepMatchColoringDefaultValue),
                EnableTracing = GetGeneralOption(dte, "EnableTracing", EnableTracingDefaultValue),
                TracingCategories = GetGeneralOption(dte, "TracingCategories", TracingCategoriesDefaultValue),
                DisableRegenerateFeatureFilePopupOnConfigChange = GetGeneralOption(dte, "DisableRegenerateFeatureFilePopupOnConfigChange", DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue),
                GenerationMode = GetGeneralOption(dte, "GenerationMode", GenerationModeDefaultValue),
                CodeBehindFileGeneratorPath = GetGeneralOption(dte, "PathToCodeBehindGeneratorExe", CodeBehindFileGeneratorPath),
                CodeBehindFileGeneratorExchangePath = GetGeneralOption(dte, "CodeBehindFileGeneratorExchangePath", CodeBehindFileGeneratorExchangePath),
                OptOutDataCollection = GetGeneralOption(dte, "OptOutDataCollection", DefaultOptOutDataCollection)
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
    }
}
