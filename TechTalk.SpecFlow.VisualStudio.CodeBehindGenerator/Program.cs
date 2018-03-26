using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<GetTestFullPathParameters, DetectGeneratedTestVersionParameters, GenerateTestFileParameters>(args)
                .MapResult(
                    (GetTestFullPathParameters opts) => GetTestFullPath(opts),
                    (DetectGeneratedTestVersionParameters opts) => DetectGeneratedTestVersion(opts),
                    (GenerateTestFileParameters opts) => GenerateTestFile(opts),
                    errs => 1);
        }

        private static int GenerateTestFile(GenerateTestFileParameters opts)
        {
            AttachToDebuggerIfNeeded(opts);

            try
            {

                var featureFileInput = JsonConvert.DeserializeObject<FeatureFileInput>(File.ReadAllText(opts.FeatureFile));
                var projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(opts.ProjectSettingsFile));

                var codeDomHelper = GenerationTargetLanguage.CreateCodeDomHelper(projectSettings.ProjectPlatformSettings.Language);


                var testGeneratorFactory = new TestGeneratorFactory();
                var testGenerator = testGeneratorFactory.CreateGenerator(projectSettings);
                var testGeneratorResult = testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());

                string outputFileContent;
                if (testGeneratorResult.Success)
                {
                    outputFileContent = testGeneratorResult.GeneratedTestCode;
                }
                else
                {
                    outputFileContent = GenerateError(testGeneratorResult, codeDomHelper);
                }

                Console.WriteLine(outputFileContent);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static int DetectGeneratedTestVersion(DetectGeneratedTestVersionParameters opts)
        {
            AttachToDebuggerIfNeeded(opts);

            try
            {
                var featureFileInput = JsonConvert.DeserializeObject<FeatureFileInput>(File.ReadAllText(opts.FeatureFile));


                var testGeneratorFactory = new TestGeneratorFactory();
                var testGenerator = testGeneratorFactory.CreateGenerator(new ProjectSettings());
                var version = testGenerator.DetectGeneratedTestVersion(featureFileInput);
                Console.WriteLine(version);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static int GetTestFullPath(GetTestFullPathParameters opts)
        {
            AttachToDebuggerIfNeeded(opts);

            try
            {
                var featureFileInput = JsonConvert.DeserializeObject<FeatureFileInput>(File.ReadAllText(opts.FeatureFile));


                var testGeneratorFactory = new TestGeneratorFactory();
                var testGenerator = testGeneratorFactory.CreateGenerator(new ProjectSettings());
                var version = testGenerator.GetTestFullPath(featureFileInput);
                Console.WriteLine(version);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static string GenerateError(TestGeneratorResult generationResult, CodeDomHelper codeDomHelper)
        {
            var errorsArray = generationResult.Errors.ToArray();

            
            return string.Join(Environment.NewLine, errorsArray.Select(e => codeDomHelper.GetErrorStatementString(e.Message)).ToArray());
        }

        private static string GenerateError(Exception ex, CodeDomHelper codeDomHelper)
        {
            TestGenerationError testGenerationError = new TestGenerationError(ex);
            
            var exceptionText = ex.Message + Environment.NewLine +
                                Environment.NewLine +
                                ex.Source + Environment.NewLine +
                                ex.StackTrace;

            var errorMessage = string.Join(Environment.NewLine, exceptionText
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(codeDomHelper.GetErrorStatementString));

            return errorMessage;
        }


        private static void AttachToDebuggerIfNeeded(CommonParameters parameters)
        {
            if (parameters.Debug)
            {
                Debugger.Break();
            }
        }
    }
}
