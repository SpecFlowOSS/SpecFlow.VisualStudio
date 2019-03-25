using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.VsIntegration.StepSuggestions
{
    public interface ISourceFilePosition
    {
        string SourceFile { get; }
        FilePosition FilePosition { get; }
    }
}