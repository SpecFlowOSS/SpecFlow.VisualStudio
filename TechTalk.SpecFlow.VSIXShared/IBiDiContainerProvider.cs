using BoDi;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public interface IBiDiContainerProvider
    {
        IObjectContainer ObjectContainer { get; }
    }
}