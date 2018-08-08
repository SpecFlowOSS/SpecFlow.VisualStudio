using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
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
        private readonly ISpecFlowConfigurationReader configurationReader;

        public VsGeneratorInfoProvider(Project project, IIdeTracer tracer, ISpecFlowConfigurationReader configurationReader)
        {
            this.project = project;
            this.tracer = tracer;
            this.configurationReader = configurationReader;
        }

        //todo: replace GeneratorConfig stuff - it is from the old SpecFlow.dll
        public virtual GeneratorInfo GetGeneratorInfo()
        {
            var generatorInfo = new GeneratorInfo();
            return generatorInfo;
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