using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public class ExtensionLoadedAnalyticsEvent : AnalyticsEventBase
    {
        public ExtensionLoadedAnalyticsEvent(string ide, DateTime utcDate, string userId, string ideVersion, string extensionVersion, IEnumerable<string> projectTargetFrameworks) : base(ide, ideVersion, utcDate, userId)
        {
            ExtensionVersion = extensionVersion;
            ProjectTargetFrameworks = projectTargetFrameworks.ToArray();
        }

        public override string EventName => "Extension loaded";
        
        public string ExtensionVersion { get; }
        public IReadOnlyList<string> ProjectTargetFrameworks { get; }
    }
}