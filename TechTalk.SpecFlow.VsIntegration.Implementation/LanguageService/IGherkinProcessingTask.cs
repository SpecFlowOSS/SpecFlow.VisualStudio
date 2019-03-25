namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IGherkinProcessingTask
    {
        void Apply();
        IGherkinProcessingTask Merge(IGherkinProcessingTask other);
    }
}