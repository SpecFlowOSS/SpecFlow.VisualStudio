using BoDi;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Services;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public static class ObjectContainerExtensions
    {
        public static void RegisterAnalyticsDefaults(this IObjectContainer container)
        {
            container.RegisterTypeAs<FileUserIdStore, IUserUniqueIdStore>();

            container.RegisterTypeAs<WindowsRegistry, IWindowsRegistry>();
            container.RegisterTypeAs<FileService, IFileService>();
            container.RegisterTypeAs<DirectoryService, IDirectoryService>();

            container.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            container.RegisterTypeAs<EnableAnalyticsChecker, IEnableAnalyticsChecker>();

            container.RegisterTypeAs<EnvironmentSpecFlowTelemetryChecker, IEnvironmentSpecFlowTelemetryChecker>();

            container.RegisterTypeAs<TelemetryClientWrapper, TelemetryClientWrapper>();
            container.RegisterTypeAs<AppInsightsEventConverter, IAppInsightsEventConverter>();
            container.RegisterTypeAs<AppInsightsAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();

            container.RegisterTypeAs<CurrentExtensionVersionProvider, ICurrentExtensionVersionProvider>();

            container.RegisterTypeAs<VisualStudioIdeInformationStore, IIdeInformationStore>();
        }
    }
}
