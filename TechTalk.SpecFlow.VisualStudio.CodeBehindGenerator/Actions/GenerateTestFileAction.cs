using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Actions
{
    class GenerateTestFileAction
    {
        private readonly Type _specFlowConfigurationHolderFieldInfo;

        public GenerateTestFileAction()
        {
            _specFlowConfigurationHolderFieldInfo = typeof(SpecFlowConfigurationHolder);
        }

        public int GenerateTestFile(GenerateTestFileParameters opts)
        {
            try
            {
                var featureFileInput = DeserializeFeatureFileInput(opts);
                var projectSettings = DeserializeProjectSettings(opts.ProjectSettingsFile);

                var codeDomHelper = GetCodeDomHelper(projectSettings);


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

                Console.WriteLine(WriteTempFile(opts, outputFileContent));

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private string WriteTempFile(GenerateTestFileParameters opts, string content)
        {
            var fileName = Path.Combine(opts.OutputDirectory, Path.GetRandomFileName());
            File.WriteAllText(fileName, content, Encoding.UTF8);
            return fileName;
        }

        private CodeDomHelper GetCodeDomHelper(ProjectSettings projectSettings)
        {
            return GenerationTargetLanguage.CreateCodeDomHelper(projectSettings.ProjectPlatformSettings.Language);
        }

        private FeatureFileInput DeserializeFeatureFileInput(GenerateTestFileParameters opts)
        {
            string featureFileContent = File.ReadAllText(opts.FeatureFile);
            var featureFileInput = JsonConvert.DeserializeObject<FeatureFileInput>(featureFileContent);
            return featureFileInput;
        }

        private ProjectSettings DeserializeProjectSettings(string projectSettingsFile)
        {
            string projectSettingsContent = File.ReadAllText(projectSettingsFile);
            var projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(projectSettingsContent);

            var projectSettingsJson = JObject.Parse(projectSettingsContent);
            var xmlString = projectSettingsJson["ConfigurationHolder"]["XmlString"].Value<string>();


            var fieldInfo = _specFlowConfigurationHolderFieldInfo.GetField("xmlString", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(projectSettings.ConfigurationHolder, xmlString);
            }

            var configSourceFieldInfo = _specFlowConfigurationHolderFieldInfo.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.FieldType.Name == "ConfigSource").SingleOrDefault();

            if (configSourceFieldInfo != null)
            {
                if (IsConfigXml(xmlString))
                {
                    configSourceFieldInfo.SetValue(projectSettings.ConfigurationHolder, 0);
                }
                else
                {
                    if (IsConfigJson(xmlString))
                    {
                        configSourceFieldInfo.SetValue(projectSettings.ConfigurationHolder, 1);
                    }
                    else
                    {
                        configSourceFieldInfo.SetValue(projectSettings.ConfigurationHolder, 2);
                    }
                }
                    
            }

            return projectSettings;
        }

        private bool IsConfigJson(string configContent)
        {
            return configContent.StartsWith("{") || configContent.StartsWith("[");
        }

        private bool IsConfigXml(string configContent)
        {
            return configContent.StartsWith("<");
        }

        private string GenerateError(TestGeneratorResult generationResult, CodeDomHelper codeDomHelper)
        {
            var errorsArray = generationResult.Errors.ToArray();


            return string.Join(Environment.NewLine, errorsArray.Select(e => codeDomHelper.GetErrorStatementString(e.Message)).ToArray());
        }

        private string GenerateError(Exception ex, CodeDomHelper codeDomHelper)
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
    }
}
