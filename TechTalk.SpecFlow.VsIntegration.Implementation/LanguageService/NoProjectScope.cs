using System;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.VsIntegration.Implementation.GherkinFileEditor;
using TechTalk.SpecFlow.VsIntegration.Implementation.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class NoProjectScope : IProjectScope
    {
        public GherkinTextBufferParser GherkinTextBufferParser { get; private set; }
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowConfiguration SpecFlowConfiguration { get; private set; }
        public GherkinDialectServices GherkinDialectServices { get; private set; }
        public IIntegrationOptionsProvider IntegrationOptionsProvider { get; private set; }
        public IIdeTracer Tracer { get; private set; }

        public event Action SpecFlowConfigurationChanged { add {} remove {} }
        public event Action GherkinDialectServicesChanged { add { } remove { } }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { return null; }
        }

        public VsStepSuggestionProvider StepSuggestionProvider
        {
            get { return null; }
        }

        public IStepDefinitionMatchService BindingMatchService
        {
            get { return null; }
        }

        public IGeneratorServices GeneratorServices
        {
            get { return null; }
        }

        public NoProjectScope(GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer, IIntegrationOptionsProvider integrationOptionsProvider)
        {
            GherkinTextBufferParser = new GherkinTextBufferParser(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer, false);
            SpecFlowConfiguration = ConfigurationLoader.GetDefault();
            GherkinDialectServices = new GherkinDialectServices(SpecFlowConfiguration.FeatureLanguage); 
            Classifications = classifications;
            IntegrationOptionsProvider = integrationOptionsProvider;
            Tracer = visualStudioTracer;
        }

        public void Dispose()
        {
            //nop
        }
    }
}