namespace TechTalk.SpecFlow.VsIntegration.Tracing.OutputWindow
{
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
