namespace TechTalk.SpecFlow.IdeIntegration.Options
{
    public interface IIntegrationOptionsProvider
    {
        IntegrationOptions GetOptions();
        void ClearCache();
    }
}