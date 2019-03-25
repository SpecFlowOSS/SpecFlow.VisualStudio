﻿using TechTalk.SpecFlow.VsIntegration.Implementation.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class GherkinScopeAnalyzer
    {
        private IProjectScope projectScope;
        private readonly IVisualStudioTracer visualStudioTracer;

        public GherkinScopeAnalyzer(IProjectScope projectScope, IVisualStudioTracer visualStudioTracer)
        {
            this.projectScope = projectScope;
            this.visualStudioTracer = visualStudioTracer;
        }

        public GherkinFileScopeChange Analyze(GherkinFileScopeChange change)
        {
            visualStudioTracer.Trace("Analyzing started", "GherkinScopeAnalyzer");
            return change; //TODO
        }
    }
}