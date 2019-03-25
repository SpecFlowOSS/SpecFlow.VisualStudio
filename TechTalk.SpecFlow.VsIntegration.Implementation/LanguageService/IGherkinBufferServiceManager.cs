using System;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IGherkinBufferServiceManager
    {
        TService GetOrCreate<TService>(ITextBuffer textBuffer, Func<TService> creator)
            where TService : class, IDisposable;
    }
}