using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    class DetectGeneratedTestVersionAction
    {
        public int DetectGeneratedTestVersion(DetectGeneratedTestVersionParameters opts)
        {
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

    }
}
