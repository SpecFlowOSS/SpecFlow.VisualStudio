using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TestWindow.Data;
using Microsoft.VisualStudio.TestWindow.Extensibility;

namespace TechTalk.SpecFlow.VsIntegration.TestRunner
{
    [Export(typeof(ITestMethodResolver))]
    public class SpecRunTestMethodResolver : ITestMethodResolver
    {
        private readonly IUnitTestStorage unitTestStorage;

        public Uri ExecutorUri { get { return SpecRunConstants.ExecutorUri; } }

        [ImportingConstructor]
        public SpecRunTestMethodResolver(IUnitTestStorage unitTestStorage)
        {
            this.unitTestStorage = unitTestStorage;
        }

        public string GetCurrentTest(string filePath, int line, int lineCharOffset)
        {
            if (filePath == null || !filePath.EndsWith(".feature", StringComparison.OrdinalIgnoreCase))
                return null;

            var testsToRun = unitTestStorage.ActiveUnitTestReader.GetAllTestsWithProperties()
                .Where(t => string.Equals(t.FilePath, filePath, StringComparison.OrdinalIgnoreCase) && t.LineNumber >= line)
                .OrderBy(t => t.LineNumber);

            var testToRun = testsToRun.FirstOrDefault();

            return testToRun != null && testToRun.ExecutorUri.Equals(SpecRunConstants.ExecutorUriString)  ? testToRun.FullyQualifiedName : null;
        }

    }
}
