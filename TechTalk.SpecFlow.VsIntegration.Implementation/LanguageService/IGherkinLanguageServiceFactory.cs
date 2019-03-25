using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IGherkinLanguageServiceFactory
    {
        GherkinLanguageService GetLanguageService(ITextBuffer textBuffer);
    }
}