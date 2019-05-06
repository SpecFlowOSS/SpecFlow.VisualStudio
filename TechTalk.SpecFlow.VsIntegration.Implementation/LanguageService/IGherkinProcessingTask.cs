namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IGherkinProcessingTask
    {
        void Apply();
        IGherkinProcessingTask Merge(IGherkinProcessingTask other);
    }
}