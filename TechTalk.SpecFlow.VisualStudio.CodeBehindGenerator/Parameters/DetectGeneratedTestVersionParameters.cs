using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters
{
    [Verb("DetectGeneratedTestVersion")]
    public class DetectGeneratedTestVersionParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }
    }
}