using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionLoadedAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionLoadedAnalyticsEvent(DateTime utcDate, string userId, string ide, string ideVersion, string extensionVersion, IEnumerable<string> projectTargetFrameworks) : base(utcDate, userId)
        {
            Ide = ide;
            IdeVersion = ideVersion;
            ExtensionVersion = extensionVersion;
            ProjectTargetFrameworks = projectTargetFrameworks.ToArray();
        }

        public override string EventName => "Extension loaded";

        public string Ide { get; }
        public string IdeVersion { get; }
        public string ExtensionVersion { get; }
        public IReadOnlyList<string> ProjectTargetFrameworks { get; }
    }
}