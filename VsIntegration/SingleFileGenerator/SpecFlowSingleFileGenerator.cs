using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Generator;
using TechTalk.SpecFlow.VsIntegration.SingleFileGenerator;
using TechTalk.SpecFlow.VsIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Utils;
using VSLangProj80;

namespace TechTalk.SpecFlow.VsIntegration
{
    [ComVisible(true)]
    [Guid("44F8C2E2-18A9-4B97-B830-6BCD0CAA161C")]
    // Must register new project type which contains the new multi target model, https://github.com/aspnet/Tooling/issues/394#issuecomment-319244129
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : SpecFlowSingleFileGeneratorBase
    {
        [Import]
        internal IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        protected override Func<GeneratorServices> GeneratorServicesProvider(Project project)
        {
            IVisualStudioTracer tracer = VsxHelper.ResolveMefDependency<IVisualStudioTracer>(ServiceProvider.GlobalProvider);
            IntegrationOptionsProvider = VsxHelper.ResolveMefDependency<IIntegrationOptionsProvider>(ServiceProvider.GlobalProvider);
            return () => new VsGeneratorServices(project, new VsSpecFlowConfigurationReader(project, tracer), tracer, IntegrationOptionsProvider);
        }
    }
}