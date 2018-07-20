﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    public class OutOfProcessTestGenerator : ITestGenerator
    {
        private readonly OutOfProcessExecutor _outOfProcessExecutor;
        private readonly ProjectSettings _projectSettings;

        public OutOfProcessTestGenerator(Info info, ProjectSettings projectSettings, IntegrationOptions integrationOptions)
        {
            _projectSettings = projectSettings;
            _outOfProcessExecutor = new OutOfProcessExecutor(info, integrationOptions);
        }

        public void Dispose()
        {
        }

        public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            string projectSettingsFile = WriteTempFile(_projectSettings);
            var featureFileInputFile = WriteTempFile(featureFileInput);

            var result = _outOfProcessExecutor.Execute(new GenerateTestFileParameters()
            {
                FeatureFile = featureFileInputFile,
                ProjectSettingsFile = projectSettingsFile,
                Debug = Debugger.IsAttached
            });

            return new TestGeneratorResult(result.Output, true);
        }

        public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
        {
            var featureFileInputFile = WriteTempFile(featureFileInput);


            var result = _outOfProcessExecutor.Execute(new DetectGeneratedTestVersionParameters()
            {
                FeatureFile = featureFileInputFile,
                Debug = Debugger.IsAttached
            });


            if (result.ExitCode > 0)
            {
                throw new Exception(result.Output);
            }

            return Version.Parse(result.Output);
        }

        public string GetTestFullPath(FeatureFileInput featureFileInput)
        {
            var featureFileInputFile = WriteTempFile(featureFileInput);


            var result = _outOfProcessExecutor.Execute(new GetTestFullPathParameters()
            {
                FeatureFile = featureFileInputFile,
                Debug = Debugger.IsAttached
            });

            if (result.ExitCode > 0)
            {
                throw new Exception(result.Output);
            }

            return result.Output;
        }

        

        private string WriteTempFile(object settings)
        {
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, JsonConvert.SerializeObject(settings));
            return fileName;
        }
    }
}