using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class ErrorInfo
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int LinePosition { get; set; }
        public Exception Exception { get; set; }

        public ErrorInfo(string message, int line, int linePosition, Exception exception)
        {
            Message = message;
            Line = line;
            LinePosition = linePosition;
            Exception = exception;
        }
    }
}