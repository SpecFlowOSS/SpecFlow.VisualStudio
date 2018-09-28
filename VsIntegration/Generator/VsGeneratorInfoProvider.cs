using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Generator
{
    internal class VsGeneratorInfoProvider : IGeneratorInfoProvider
    {
        private static readonly string[] probingPaths = new[]
                                                            {
                                                                @".",
                                                                @"tools",
                                                                @"..\tools",
                                                                @"..\..\tools",
                                                            };
        const string generatorAssemblyName = "TechTalk.SpecFlow.Generator";

        protected readonly Project project;
        protected readonly IIdeTracer tracer;
        private readonly IConfigurationReader configurationReader;

        public VsGeneratorInfoProvider(Project project, IIdeTracer tracer, IConfigurationReader configurationReader)
        {
            this.project = project;
            this.tracer = tracer;
            this.configurationReader = configurationReader;
        }

        public virtual GeneratorInfo GetGeneratorInfo()
        {
            tracer.Trace("Discovering generator information...", "VsGeneratorInfoProvider");

            var specflowGeneratorConfig = GenSpecFlowGeneratorConfig();

            try
            {
                var specflowGeneratorInfo = new GeneratorInfo() { UsesPlugins = specflowGeneratorConfig.UsesPlugins };
                if (DetectFromConfig(specflowGeneratorInfo, specflowGeneratorConfig))
                    return specflowGeneratorInfo;

                if (!DetectFromRuntimeReference(specflowGeneratorInfo))
                    tracer.Trace("Unable to detect generator path", "VsGeneratorInfoProvider");
                
                return specflowGeneratorInfo;
            }
            catch (Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return null;
            }
        }

        private SpecFlowGeneratorConfiguration GenSpecFlowGeneratorConfig()
        {
            try
            {
                var specflowGeneratorConfig = new SpecFlowGeneratorConfiguration();

                var configurationHolder = configurationReader.ReadConfiguration();
                switch (configurationHolder.ConfigSource)
                {
                    case ConfigSource.AppConfig:
                        var appConfigFormat = configurationHolder.TransformConfigurationToOldHolder();
                        var oldGeneratorConfig = new GeneratorConfigurationProvider().LoadConfiguration(appConfigFormat).GeneratorConfiguration;

                        specflowGeneratorConfig.GeneratorPath = oldGeneratorConfig.GeneratorPath;
                        specflowGeneratorConfig.UsesPlugins = oldGeneratorConfig.UsesPlugins;
                        break;
                    case ConfigSource.Json:
                        var defaultSpecFlowConfiguration = ConfigurationLoader.GetDefault();
                        var specflowLoader = new ConfigurationLoader();
                        var jsonConfig = specflowLoader.Load(defaultSpecFlowConfiguration, configurationHolder);

                        specflowGeneratorConfig.GeneratorPath = jsonConfig.GeneratorPath;
                        specflowGeneratorConfig.UsesPlugins = jsonConfig.UsesPlugins;
                        break;
                }

                return specflowGeneratorConfig;
            }
            catch (Exception exception)
            {
                tracer.Trace("Config load error: " + exception, "VsGeneratorInfoProvider");
                return new SpecFlowGeneratorConfiguration();
            }
        }

        private bool DetectFromConfig(GeneratorInfo generatorInfo, SpecFlowGeneratorConfiguration generatorConfiguration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(generatorConfiguration.GeneratorPath))
                    return false;

                var generatorFolder = Path.GetFullPath(
                    Path.Combine(VsxHelper.GetProjectFolder(project), generatorConfiguration.GeneratorPath));

                tracer.Trace("Generator is configured to be at " + generatorFolder, "VsGeneratorInfoProvider");
                return DetectFromFolder(generatorInfo, generatorFolder);
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return false;
            }
        }

        private bool DetectFromRuntimeReference(GeneratorInfo generatorInfo)
        {
            var specFlowRef = VsxHelper.GetReference(project, "TechTalk.SpecFlow");
            if (specFlowRef == null)
                return false;

            var specFlowRefPath = specFlowRef.Path;
            if (string.IsNullOrWhiteSpace(specFlowRefPath))
                return false;

            string runtimeFolder = Path.GetDirectoryName(specFlowRefPath);
            if (runtimeFolder == null)
                return false;

            tracer.Trace("Runtime found at " + runtimeFolder, "VsGeneratorInfoProvider");

            return probingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Any(probingPath => DetectFromFolder(generatorInfo, probingPath));
        }

        private bool DetectFromFolder(GeneratorInfo generatorInfo, string generatorFolder)
        {
            const string generatorAssemblyFileName = generatorAssemblyName + ".dll";

            var generatorPath = Path.Combine(generatorFolder, generatorAssemblyFileName);
            if (!File.Exists(generatorPath))
                return false;

            tracer.Trace("Generator found at " + generatorPath, "VsGeneratorInfoProvider");
            var fileVersion = FileVersionInfo.GetVersionInfo(generatorPath);
            if (fileVersion.FileVersion == null)
            {
                tracer.Trace("Could not detect generator version", "VsGeneratorInfoProvider");
                return false;
            }

            generatorInfo.GeneratorAssemblyVersion = new Version(fileVersion.FileVersion);
            generatorInfo.GeneratorFolder = Path.GetDirectoryName(generatorPath);

            return true;
        }
    }
}