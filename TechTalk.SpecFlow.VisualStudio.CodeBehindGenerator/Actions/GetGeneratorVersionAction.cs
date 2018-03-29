using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Actions
{
    class GetGeneratorVersionAction
    {
        public int GetGeneratorVersion(GetGeneratorVersionParameters opts)
        {
            try
            {
                var testGeneratorFactory = new TestGeneratorFactory();
                Console.WriteLine(testGeneratorFactory.GetGeneratorVersion().ToString());
                
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
