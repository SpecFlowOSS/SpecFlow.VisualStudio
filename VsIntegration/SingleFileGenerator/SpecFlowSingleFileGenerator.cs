using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.VsIntegration.Generator;
using TechTalk.SpecFlow.VsIntegration.SingleFileGenerator;
using TechTalk.SpecFlow.VsIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Utils;
using VSLangProj80;

namespace TechTalk.SpecFlow.VsIntegration
{
    [ComVisible(true)]
    [Guid("44F8C2E2-18A9-4B97-B830-6BCD0CAA161C")]
    [CodeGeneratorRegistrationWithFileExtension(typeof(SpecFlowSingleFileGenerator), "C# SpecFlow Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistrationWithFileExtension(typeof(SpecFlowSingleFileGenerator), "VB.NET SpecFlow Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistrationWithFileExtension(typeof(SpecFlowSingleFileGenerator), "Silverlight SpecFlow Generator", GuidList.vsContextGuidSilverlightProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : SpecFlowSingleFileGeneratorBase
    {
        protected override Func<GeneratorServices> GeneratorServicesProvider(Project project)
        {
            IVisualStudioTracer tracer = VsxHelper.ResolveMefDependency<IVisualStudioTracer>(ServiceProvider.GlobalProvider);
            return () => new VsGeneratorServices(project, new VsSpecFlowConfigurationReader(project, tracer), tracer);
        }
    }
}