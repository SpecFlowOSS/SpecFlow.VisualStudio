using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Generator.AppDomain;
using TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Generator
{
    internal class VsGeneratorServices : RemoteGeneratorServices
    {
        protected readonly Project project;
        private readonly IConfigurationReader configurationReader;
        private readonly IIntegrationOptionsProvider _integrationOptionsProvider;


        public VsGeneratorServices(Project project, IConfigurationReader configurationReader, IIdeTracer tracer,
            IIntegrationOptionsProvider integrationOptionsProvider) : base( //TODO: load dependencies through DI
            new TestGeneratorFactory(), 
            new RemoteAppDomainTestGeneratorFactory(tracer), 
            new OutOfProcessTestGeneratorFactory(tracer, integrationOptionsProvider.GetOptions()), 
            new VsGeneratorInfoProvider(project, tracer, configurationReader), 
            tracer, false)
        {
            this.project = project;
            this.configurationReader = configurationReader;
            _integrationOptionsProvider = integrationOptionsProvider;

            UseOutOfProcess = _integrationOptionsProvider.GetOptions().GenerationMode == GenerationMode.OutOfProcess;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            tracer.Trace("Discover project settings", "VsGeneratorServices");

            ProjectPlatformSettings projectPlatformSettings;
            var tergetLanguage = VsProjectScope.GetTargetLanguage(project);
            switch (tergetLanguage)
            {
                case ProgrammingLanguage.CSharp:
                    projectPlatformSettings = new ProjectPlatformSettings
                    {
                        Language = GenerationTargetLanguage.CSharp,
                        LanguageVersion = new Version("3.0"),
                        Platform = GenerationTargetPlatform.DotNet,
                        PlatformVersion = new Version("3.5"),
                    };
                    break;
                case ProgrammingLanguage.VB:
                    projectPlatformSettings = new ProjectPlatformSettings
                    {
                        Language = GenerationTargetLanguage.VB,
                        LanguageVersion = new Version("9.0"),
                        Platform = GenerationTargetPlatform.DotNet,
                        PlatformVersion = new Version("3.5"),
                    };
                    break;
                default:
                    throw new NotSupportedException("target language not supported");
            }

            var configurationHolder = configurationReader.ReadConfiguration();
            return new ProjectSettings
                       {
                           ProjectName = Path.GetFileNameWithoutExtension(project.FullName),
                           AssemblyName = VsxHelper.GetProjectAssemblyName(project),
                           ProjectFolder = VsxHelper.GetProjectFolder(project),
                           DefaultNamespace = VsxHelper.GetProjectDefaultNamespace(project),
                           ProjectPlatformSettings = projectPlatformSettings,
                           ConfigurationHolder = configurationHolder.TransformConfigurationToOldHolder()
                       };
        }
    }
}
