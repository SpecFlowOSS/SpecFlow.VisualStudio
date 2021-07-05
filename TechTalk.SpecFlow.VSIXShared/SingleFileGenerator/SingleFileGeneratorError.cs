namespace TechTalk.SpecFlow.VsIntegration.Implementation.SingleFileGenerator
{
    public class SingleFileGeneratorError
    {
        /// <summary>
        /// Zero-based line index.
        /// </summary>
        public readonly int Line;
        /// <summary>
        /// Zero-based position index.
        /// </summary>
        public readonly int LinePosition;
        public readonly string Message;

        public SingleFileGeneratorError(string message) : this(0, 0, message)
        {
        }

        public SingleFileGeneratorError(int line, int linePosition, string message)
        {
            Line = line;
            LinePosition = linePosition;
            Message = message;
        }
    }
}