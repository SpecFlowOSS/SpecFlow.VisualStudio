using System;
using System.Runtime.Serialization;

namespace gherkin.lexer
{
    [Serializable]
    public class LexingError : Exception
    {
        public LexingError()
        {
        }

        public LexingError(string message) : base(message)
        {
        }

        public LexingError(string message, Exception inner) : base(message, inner)
        {
        }

        protected LexingError(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
