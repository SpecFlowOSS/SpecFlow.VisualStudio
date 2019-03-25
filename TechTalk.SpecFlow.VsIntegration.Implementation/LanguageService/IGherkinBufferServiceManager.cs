using System;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal interface IGherkinBufferServiceManager
    {
        TService GetOrCreate<TService>(ITextBuffer textBuffer, Func<TService> creator)
            where TService : class, IDisposable;
    }
}