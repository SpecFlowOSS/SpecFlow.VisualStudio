using System;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class EnableAnalyticsChecker : IEnableAnalyticsChecker
    {
        private readonly IIntegrationOptionsProvider _integrationOptionsProvider;
        private readonly IEnvironmentSpecFlowTelemetryChecker _environmentSpecFlowTelemetryChecker;

        public EnableAnalyticsChecker(IIntegrationOptionsProvider integrationOptionsProvider, IEnvironmentSpecFlowTelemetryChecker environmentSpecFlowTelemetryChecker)
        {
            _integrationOptionsProvider = integrationOptionsProvider;
            _environmentSpecFlowTelemetryChecker = environmentSpecFlowTelemetryChecker;
        }

        public bool IsEnabled()
        {
            var options = _integrationOptionsProvider.GetOptions();
            var isSpecFlowTelemetryEnabled = _environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled();
            return options.OptOutDataCollection && isSpecFlowTelemetryEnabled;
        }
    }
}
