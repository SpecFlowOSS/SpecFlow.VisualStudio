using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IGherkinLanguageServiceFactory
    {
        GherkinLanguageService GetLanguageService(ITextBuffer textBuffer);
    }
}