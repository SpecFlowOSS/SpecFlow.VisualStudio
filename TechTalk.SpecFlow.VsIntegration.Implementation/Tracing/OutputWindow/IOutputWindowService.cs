namespace TechTalk.SpecFlow.VsIntegration.Implementation.Tracing.OutputWindow
{
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
