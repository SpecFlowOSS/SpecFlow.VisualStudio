using System;
using System.IO;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Actions
{
    class DetectGeneratedTestVersionAction
    {
        public int DetectGeneratedTestVersion(DetectGeneratedTestVersionParameters opts)
        {
            try
            {
                var featureFileInput = JsonConvert.DeserializeObject<FeatureFileInput>(File.ReadAllText(opts.FeatureFile));


                var testGeneratorFactory = new TestGeneratorFactory();
                var testGenerator = testGeneratorFactory.CreateGenerator(new ProjectSettings(), Array.Empty<GeneratorPluginInfo>());
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

    }
}
