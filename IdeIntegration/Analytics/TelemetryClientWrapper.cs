using System;
using Microsoft.ApplicationInsights;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    // this class serves only to prevent triggering BoDi's constructor policy to take the one with the most parameters.
    public class TelemetryClientWrapper
    {
        private readonly Lazy<TelemetryClient> _telemetryClient;

        public TelemetryClientWrapper(IUserUniqueIdStore userUniqueIdStore)
        {
            _telemetryClient = new Lazy<TelemetryClient>(() => GetTelemetryClient(userUniqueIdStore));
        }

        public TelemetryClient TelemetryClient => _telemetryClient.Value;

        private TelemetryClient GetTelemetryClient(IUserUniqueIdStore userUniqueIdStore)
        {
            var userUniqueId = userUniqueIdStore.GetUserId();
            return new TelemetryClient
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
    }
}
