using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public interface IOutOfProcessTestGeneratorFactory : ITestGeneratorFactory
    {
        void Setup(string newGeneratorFolder);
        void EnsureInitialized();
    }
}