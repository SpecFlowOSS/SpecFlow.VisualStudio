using Microsoft.ApplicationInsights;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    // this class serves only to prevent triggering BoDi's constructor policy to take the one with the most parameters.
    public class TelemetryClientWrapper
    {
        public TelemetryClientWrapper(IUserUniqueIdStore userUniqueIdStore)
        {
            var userUniqueId = userUniqueIdStore.Get();
            TelemetryClient = new TelemetryClient
            {
                Context =
                {
                    User =
                    {
                        Id = userUniqueId,
                        AccountId = userUniqueId
                    },
                },
            };
        }

        public TelemetryClient TelemetryClient { get; private set; }
    }
}
