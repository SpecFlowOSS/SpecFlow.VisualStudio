using System;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    public static class SpecRunConstants
    {
        public const string ExecutorUriString = "executor://specrun.com/executor_v1.2";
        public static Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}