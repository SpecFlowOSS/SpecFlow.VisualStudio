namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal abstract class KeywordLine : IKeywordLine
    {
        public string Keyword { get; private set; }
        public string Text { get; private set; }
        public int BlockRelativeLine { get; private set; }

        protected KeywordLine(string keyword, string text, int blockRelativeLine)
        {
            Keyword = keyword;
            Text = text;
            BlockRelativeLine = blockRelativeLine;
        }
    }
}