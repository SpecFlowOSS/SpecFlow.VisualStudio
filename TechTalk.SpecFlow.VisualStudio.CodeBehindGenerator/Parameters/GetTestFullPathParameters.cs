using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters
{
    [Verb("GetTestFullPath")]
    public class GetTestFullPathParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }
    }
}
