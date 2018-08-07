using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters
{
    public class CommonParameters
    {
        [Option]
        public bool Debug { get; set; }

        [Option]
        public string OutputDirectory { get; set; }
    }
}