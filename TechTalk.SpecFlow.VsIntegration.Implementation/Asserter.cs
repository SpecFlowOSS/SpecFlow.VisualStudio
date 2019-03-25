using System;
using System.Diagnostics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public class Asserter
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException("Assertion fauiled: " + message);
        }
    }
}