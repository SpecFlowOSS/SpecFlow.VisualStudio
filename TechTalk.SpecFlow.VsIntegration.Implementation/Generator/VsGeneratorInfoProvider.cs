using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Generator
{
    internal class VsGeneratorInfoProvider : IGeneratorInfoProvider
    {
        private const string GeneratorAssemblyName = "TechTalk.SpecFlow.Generator";

        private static readonly string[] ProbingPaths = 
        {
            @".",
            @"tools",
            @"..\tools",
            @"..\..\tools",
        };

        private readonly Project _project;
        private readonly IIdeTracer _tracer;
        private readonly IConfigurationReader _configurationReader;

        public VsGeneratorInfoProvider(Project project, IIdeTracer tracer, IConfigurationReader configurationReader)
        {
            _project = project;
            _tracer = tracer;
            _configurationReader = configurationReader;
        }

        public virtual GeneratorInfo GetGeneratorInfo()
        {
            _tracer.Trace("Discovering generator information...", "VsGeneratorInfoProvider");

            var specflowGeneratorConfig = GenSpecFlowGeneratorConfig();

            try
            {
                var specflowGeneratorInfo = new GeneratorInfo {  };
                if (DetectFromConfig(specflowGeneratorInfo, specflowGeneratorConfig))
                {
                    return specflowGeneratorInfo;
                }

                if (!DetectFromRuntimeReference(specflowGeneratorInfo))
                {
                    _tracer.Trace("Unable to detect generator path", "VsGeneratorInfoProvider");
                }
                
                return specflowGeneratorInfo;
            }
            catch (Exception exception)
            {
                _tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return null;
            }
        }

        private SpecFlowGeneratorConfiguration GenSpecFlowGeneratorConfig()
        {
            try
            {

                var specflowGeneratorConfig = new SpecFlowGeneratorConfiguration();

                var configurationHolder = _configurationReader.ReadConfiguration();
                switch (configurationHolder.ConfigSource)
                {
                    case ConfigSource.AppConfig:
                        //TODO review
                        var appConfigFormat = configurationHolder.TransformConfigurationToOldHolder();
                        //var oldGeneratorConfig = new GeneratorConfigurationProvider().LoadConfiguration(appConfigFormat).GeneratorConfiguration;

                        //specflowGeneratorConfig.GeneratorPath = oldGeneratorConfig.GeneratorPath;
                        //specflowGeneratorConfig.UsesPlugins = oldGeneratorConfig.UsesPlugins;
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
                _tracer.Trace("Config load error: " + exception, "VsGeneratorInfoProvider");
                return new SpecFlowGeneratorConfiguration();
            }
        }

        private bool DetectFromConfig(GeneratorInfo generatorInfo, SpecFlowGeneratorConfiguration generatorConfiguration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(generatorConfiguration.GeneratorPath))
                {
                    return false;
                }

                string generatorFolder = Path.GetFullPath(
                    Path.Combine(VsxHelper.GetProjectFolder(_project), generatorConfiguration.GeneratorPath));

                _tracer.Trace("Generator is configured to be at " + generatorFolder, "VsGeneratorInfoProvider");
                return DetectFromFolder(generatorInfo, generatorFolder);
            }
            catch(Exception exception)
            {
                _tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return false;
            }
        }

        private bool DetectFromRuntimeReference(GeneratorInfo generatorInfo)
        {
            var specFlowRef = VsxHelper.GetReference(_project, "TechTalk.SpecFlow");
            if (specFlowRef == null)
            {
                return false;
            }

            string specFlowRefPath = specFlowRef.Path;
            if (string.IsNullOrWhiteSpace(specFlowRefPath))
            {
                return false;
            }

            string runtimeFolder = Path.GetDirectoryName(specFlowRefPath);
            if (runtimeFolder == null)
            {
                return false;
            }

            _tracer.Trace("Runtime found at " + runtimeFolder, "VsGeneratorInfoProvider");

            return ProbingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Any(probingPath => DetectFromFolder(generatorInfo, probingPath));
        }

        private bool DetectFromFolder(GeneratorInfo generatorInfo, string generatorFolder)
        {
            const string generatorAssemblyFileName = GeneratorAssemblyName + ".dll";

            string generatorPath = Path.Combine(generatorFolder, generatorAssemblyFileName);
            if (!File.Exists(generatorPath))
            {
                return false;
            }

            _tracer.Trace("Generator found at " + generatorPath, "VsGeneratorInfoProvider");
            var fileVersion = FileVersionInfo.GetVersionInfo(generatorPath);
            if (fileVersion.FileVersion == null)
            {
                _tracer.Trace("Could not detect generator version", "VsGeneratorInfoProvider");
                return false;
            }

            generatorInfo.GeneratorAssemblyVersion = new Version(fileVersion.FileVersion);
            generatorInfo.GeneratorFolder = Path.GetDirectoryName(generatorPath);

            return true;
        }
    }
}
