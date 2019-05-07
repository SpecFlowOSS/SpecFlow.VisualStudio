namespace TechTalk.SpecFlow.IdeIntegration.Options
{
    public class IntegrationOptions
    {
        public bool EnableSyntaxColoring { get; set; }
        public bool EnableOutlining { get; set; }
        public bool EnableIntelliSense { get; set; }
        public bool LimitStepInstancesSuggestions { get; set; }
        public int MaxStepInstancesSuggestions { get; set; }
        public bool EnableAnalysis { get; set; }
        public bool EnableTableAutoFormat { get; set; }
        public bool EnableStepMatchColoring { get; set; }
        public bool EnableTracing { get; set; }
        public string TracingCategories { get; set; }
        public bool DisableRegenerateFeatureFilePopupOnConfigChange { get; set; }
        public GenerationMode GenerationMode { get; set; }
        public string CodeBehindFileGeneratorPath { get; set; }
        public string CodeBehindFileGeneratorExchangePath { get; set; }
        public bool LegacyEnableSpecFlowSingleFileGeneratorCustomTool { get; set; }
        public bool OptOutDataCollection { get; set; }
        public bool NormalizeLineBreaks { get; set; }
        public int LineBreaksBeforeScenario { get; set; }
        public int LineBreaksBeforeExamples { get; set; }
        public bool UseTabsForIndent { get; set; }
        public int FeatureIndent { get; set; }
        public int ScenarioIndent { get; set; }
        public int StepIndent { get; set; }
        public int TableIndent { get; set; }
        public int MultilineIndent { get; set; }
        public int ExampleIndent { get; set; }
    }

    public enum GenerationMode
    {
        OutOfProcess = 1
    }
}