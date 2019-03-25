namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public interface IKeywordLine
    {
        /// <summary>
        /// The keyword as it was specified in the file, including the tailing colon and space.
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// The text of the line as a direct concatenation of the <see cref="Keyword"/> in the file (no space trimming in front). 
        /// </summary>
        string Text { get; }

        /// <summary>
        /// A line number relative to <see cref="IScenarioBlock.KeywordLine"/> specifying the this line.
        /// </summary>
        int BlockRelativeLine { get; }
    }
}