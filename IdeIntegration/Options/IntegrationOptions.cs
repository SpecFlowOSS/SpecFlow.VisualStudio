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
        public TestRunnerTool TestRunnerTool { get; set; }
        public bool DisableRegenerateFeatureFilePopupOnConfigChange { get; set; }
        public GenerationMode GenerationMode { get; set; }
        public string CodeBehindFileGeneratorPath { get; set; }
        public string CodeBehindFileGeneratorExchangePath { get; set; }
        public bool OptOutDataCollection { get; set; }
    }

    public enum TestRunnerTool
    {
        Auto = 0,
        ReSharper = 1,
        VisualStudio2010MsTest = 2,
        SpecRun = 3,
        //TestDrivenDotNet = 4,
        ReSharper5 = 5,
        VisualStudio2012 = 6,
    }

    public enum GenerationMode
    {
        OutOfProcess = 1
    }
}