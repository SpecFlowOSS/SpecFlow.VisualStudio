using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class OutOfProcessTestGenerator : ITestGenerator
    {
        private const string ExeName = "TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe";
        private readonly Info _info;
        private readonly ProjectSettings _projectSettings;

        public OutOfProcessTestGenerator(Info info, ProjectSettings projectSettings)
        {
            _info = info;
            _projectSettings = projectSettings;
        }

        public void Dispose()
        {
        }

        public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            string projectSettingsFile = WriteTempFile(_projectSettings);
            var featureFileInputFile = WriteTempFile(featureFileInput);

            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(new GenerateTestFileParameters()
            {
                FeatureFile = featureFileInputFile,
                ProjectSettingsFile = projectSettingsFile,
                Debug = Debugger.IsAttached
            });

            var processHelper = new ProcessHelper();

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, ExeName, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            return new TestGeneratorResult(outputFileContent, true);
        }

        private string WriteTempFile(object settings)
        {
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, JsonConvert.SerializeObject(settings));
            return fileName;
        }

        public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
        {
            var featureFileInputFile = WriteTempFile(featureFileInput);

            var processHelper = new ProcessHelper();

            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(
                new DetectGeneratedTestVersionParameters()
                {
                    FeatureFile = featureFileInputFile,
                    Debug = Debugger.IsAttached
                });

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, ExeName, commandLineParameters);
            var outputFileContent = processHelper.ConsoleOutput;

            if (exitCode > 0)
            {
                throw new Exception(outputFileContent);
            }

            return Version.Parse(outputFileContent);
        }

        public string GetTestFullPath(FeatureFileInput featureFileInput)
        {
            var featureFileInputFile = WriteTempFile(featureFileInput);

            var processHelper = new ProcessHelper();

            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(new GetTestFullPathParameters()
            {
                FeatureFile = featureFileInputFile,
                Debug = Debugger.IsAttached
            });

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, ExeName, commandLineParameters);
            var outputFileContent = processHelper.ConsoleOutput;

            if (exitCode > 0)
            {
                throw new Exception(outputFileContent);
            }

            return outputFileContent;
        }
    }
}