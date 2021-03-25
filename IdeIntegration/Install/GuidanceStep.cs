namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class GuidanceStep
    {
        public GuidanceNotification UserLevel { get; }

        public int? UsageDays { get; }

        public string Url { get; }

        public GuidanceStep(GuidanceNotification userLevel, int? usageDays, string url)
        {
            UserLevel = userLevel;
            UsageDays = usageDays;
            Url = url;
        }
    }
}
