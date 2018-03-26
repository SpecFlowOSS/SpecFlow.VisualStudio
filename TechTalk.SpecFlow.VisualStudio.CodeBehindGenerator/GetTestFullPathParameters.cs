using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    public class CommonParameters
    {
        [Option]
        public bool Debug { get; set; }
    }


    [Verb("GetTestFullPath")]
    public class GetTestFullPathParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }
    }


    [Verb("DetectGeneratedTestVersion")]
    public class DetectGeneratedTestVersionParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }
    }

    [Verb("GenerateTestFile")]
    public class GenerateTestFileParameters : CommonParameters
    {
        [Option]
        public string FeatureFile { get; set; }

        [Option]
        public string ProjectSettingsFile { get; set; }
    }
}
