using System.Collections.Generic;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class GuidanceConfiguration
    {
        public static readonly GuidanceStep Installation =
            new GuidanceStep(GuidanceNotification.AfterInstall, null, @"https://specflow.org/welcome-to-specflow/");
    
        public static readonly GuidanceStep Upgrade = 
            new GuidanceStep(GuidanceNotification.Upgrade, null, @"https://specflow.org/welcome-to-specflow-visual-studio-integration-v2019-0/");

        public static readonly List<GuidanceStep> UsageSequence = new List<GuidanceStep>
        {
            new GuidanceStep (GuidanceNotification.TwoDayUsage, 2, "https://specflow.org/ide-onboarding-two-days/" ),
            new GuidanceStep (GuidanceNotification.FiveDayUsage, 5, "https://specflow.org/vs-onboarding-five-days/" ),
            new GuidanceStep (GuidanceNotification.TenDayUsage, 10, "https://specflow.org/beyond-the-basics/" ),
            new GuidanceStep (GuidanceNotification.TwentyDayUsage, 20, null ),
            new GuidanceStep (GuidanceNotification.HundredDayUsage, 100, "https://specflow.org/experienced/" ),
            new GuidanceStep (GuidanceNotification.TwoHundredDayUsage, 200, "https://specflow.org/veteran/")
        };
    }
}
