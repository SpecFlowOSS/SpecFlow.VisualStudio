using System.Collections.Generic;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface IGuidanceConfiguration
    {
        GuidanceStep Installation { get; }

        GuidanceStep Upgrade { get; }

        IEnumerable<GuidanceStep> UsageSequence { get; }
    }
}
