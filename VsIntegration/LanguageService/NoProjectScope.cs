﻿using System;
using System.Globalization;
using Gherkin;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.VsIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.GherkinFileEditor;
using TechTalk.SpecFlow.VsIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal class NoProjectScope : IProjectScope
    {
        public GherkinTextBufferParser GherkinTextBufferParser { get; private set; }
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
        public GherkinDialectServices GherkinDialectServices { get; private set; }
        public IIntegrationOptionsProvider IntegrationOptionsProvider { get; private set; }
        public IIdeTracer Tracer { get; private set; }

        public event Action SpecFlowProjectConfigurationChanged { add {} remove {} }
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
            SpecFlowProjectConfiguration = new SpecFlowProjectConfiguration();
            GherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage); 
            Classifications = classifications;
            IntegrationOptionsProvider = integrationOptionsProvider;
            Tracer = visualStudioTracer;
        }

        public void Dispose()
        {
            //nop
        }
    }

    public class GherkinDialectServices
    {
        public GherkinDialectServices(CultureInfo featureLanguage)
        {
            DefaultLanguage = featureLanguage;
        }

        public CultureInfo DefaultLanguage { get; private set; }

        public GherkinDialect GetDefaultDialect()
        {
            return new GherkinDialectProvider(DefaultLanguage.TwoLetterISOLanguageName).DefaultDialect;
        }
    }
}