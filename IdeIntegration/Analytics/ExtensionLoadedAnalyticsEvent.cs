using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class ExtensionLoadedAnalyticsEvent : IAnalyticsEvent
    {
        public ExtensionLoadedAnalyticsEvent(DateTime utcDate, Guid userId, string ide, string ideVersion, string extensionVersion, IEnumerable<string> projectTargetFrameworks)
        {
            UtcDate = utcDate;
            UserId = userId;
            Ide = ide;
            IdeVersion = ideVersion;
            ExtensionVersion = extensionVersion;
            ProjectTargetFrameworks = projectTargetFrameworks.ToArray();
        }

        public DateTime UtcDate { get; private set; }

        public Guid UserId { get; private set; }

        public string Ide { get; private set; }

        public string IdeVersion { get; private set; }

        public string ExtensionVersion { get; private set; }

        public IReadOnlyList<string> ProjectTargetFrameworks { get; private set; }
    }
}
