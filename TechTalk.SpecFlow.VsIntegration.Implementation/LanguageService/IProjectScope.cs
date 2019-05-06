using System;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.VsIntegration.Implementation.GherkinFileEditor;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IProjectScope : IDisposable
    {
        GherkinTextBufferParser GherkinTextBufferParser { get; }
        GherkinScopeAnalyzer GherkinScopeAnalyzer { get; }
        GherkinDialectServices GherkinDialectServices { get; }
        GherkinFileEditorClassifications Classifications { get; }
        GherkinProcessingScheduler GherkinProcessingScheduler { get; }
        SpecFlowConfiguration SpecFlowConfiguration { get; }
        VsStepSuggestionProvider StepSuggestionProvider { get; }
        IStepDefinitionMatchService BindingMatchService { get; }
        IIntegrationOptionsProvider IntegrationOptionsProvider { get; }
        IGeneratorServices GeneratorServices { get; }
        IIdeTracer Tracer { get; }

        event Action SpecFlowConfigurationChanged;
        event Action GherkinDialectServicesChanged;
    }
}