using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    [Verb("DetectGeneratedTestVersion")]
    public class DetectGeneratedTestVersionParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }
    }
}