namespace TechTalk.SpecFlow.IdeIntegration.Options
{
    public class IntegrationOptions
    {
        public bool EnableSyntaxColoring { get; set; }
        public bool EnableOutlining { get; set; }
        public bool EnableIntelliSense { get; set; }
        public bool EnableAnalysis { get; set; }
        public bool EnableTableAutoFormat { get; set; }
        public bool EnableStepMatchColoring { get; set; }
        public bool EnableTracing { get; set; }
        public string TracingCategories { get; set; }
        public TestRunnerTool TestRunnerTool { get; set; }
        public bool DisableRegenerateFeatureFilePopupOnConfigChange { get; set; }
        public GenerationMode GenerationMode { get; set; }
        public bool NormalizeLineBreaks { get; set; }
        public int LineBreaksBeforeStep { get; set; }
        public int LineBreaksBeforeScenario { get; set; }
        public int LineBreaksBeforeExamples { get; set; }
        public int LineBreaksBeforeFeature { get; set; }
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
        AppDomain = 0,
        OutOfProcess = 1
    }
}