using System;
using System.Linq;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class InstallServices
    {
        private readonly IIdeTracer tracer;
        private readonly IBrowserNotificationService notificationService;
        private readonly IFileAssociationDetector fileAssociationDetector;
        private readonly IStatusAccessor statusAccessor;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;
        private readonly ICurrentExtensionVersionProvider _currentExtensionVersionProvider;
        private readonly IDevBuildChecker _devBuildChecker;
        private readonly IGuidanceConfiguration _guidanceConfiguration;

        public IdeIntegration IdeIntegration { get; private set; }
        private Version CurrentVersion => _currentExtensionVersionProvider.GetCurrentExtensionVersion();

    
        private bool IsDevBuild => _devBuildChecker.IsDevBuild();

        public InstallServices(IBrowserNotificationService notificationService, IIdeTracer tracer, IFileAssociationDetector fileAssociationDetector, IStatusAccessor statusAccessor, IAnalyticsTransmitter analyticsTransmitter, ICurrentExtensionVersionProvider currentExtensionVersionProvider, IDevBuildChecker devBuildChecker, IGuidanceConfiguration guidanceConfiguration)
        {
            this.notificationService = notificationService;
            this.tracer = tracer;
            this.fileAssociationDetector = fileAssociationDetector;
            this.statusAccessor = statusAccessor;
            _analyticsTransmitter = analyticsTransmitter;
            _currentExtensionVersionProvider = currentExtensionVersionProvider;
            _devBuildChecker = devBuildChecker;
            _guidanceConfiguration = guidanceConfiguration;
            IdeIntegration = IdeIntegration.Unknown;
        }

        public void OnPackageLoad(IdeIntegration ideIntegration)
        {
            IdeIntegration = ideIntegration;

            if (IsDevBuild)
            {
                tracer.Trace("Running on 'dev' version on {0}", this, ideIntegration);
            }

            var today = DateTime.Today;
            var status = GetInstallStatus();

            if (!status.IsInstalled)
            {
                // new user
                if (ShowNotification(_guidanceConfiguration.Installation))
                {
                    _analyticsTransmitter.TransmitExtensionInstalledEvent();

                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;
                    status.LastUsedDate = today;

                    UpdateStatus(status);
                    CheckFileAssociation();
                }
            }
            else if (status.InstalledVersion < CurrentVersion)
            {
                //upgrading user   
                CheckFileAssociation();
            }

            _analyticsTransmitter.TransmitExtensionLoadedEvent();
        }

        private void CheckFileAssociation()
        {
            var isAssociated = fileAssociationDetector.IsAssociated();
            if (isAssociated != null && !isAssociated.Value)
            {
                tracer.Trace(".feature is not associated to SpecFlow", this);
                if (!fileAssociationDetector.SetAssociation())
                    tracer.Trace("Unable to associate .feature to SpecFlow", this);
            }
        }

        public void OnPackageUsed(bool isSpecRunUsed)
        {
            if (IsDevBuild)
                tracer.Trace("Package used", this);

            UpdateUsageOfExtension(isSpecRunUsed);
        }

        private void UpdateUsageOfExtension(bool isSpecRunUsed)
        {
            var today = DateTime.Today;
            var status = GetInstallStatus();

            if (!status.IsInstalled)
                return;

            if (status.LastUsedDate != today)
            {
                //a shiny new day with SpecFlow
                status.UsageDays++;
                status.LastUsedDate = today;
                UpdateStatus(status);
            }

            if (status.InstalledVersion < CurrentVersion)
            {
                //upgrading user   
                if (ShowNotification(_guidanceConfiguration.Upgrade))
                {
                    _analyticsTransmitter.TransmitExtensionUpgradedEvent(status.InstalledVersion.ToString());

                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;

                    UpdateStatus(status);
                }
            }
            else
            {
                var guidance = _guidanceConfiguration.UsageSequence
                    .FirstOrDefault(i => status.UsageDays >= i.UsageDays && status.UserLevel < (int)i.UserLevel);
                
                if (guidance?.UsageDays != null)
                {
                    if (guidance.Url == null || ShowNotification(guidance))
                    {
                        _analyticsTransmitter.TransmitExtensionUsage(guidance.UsageDays.Value);

                        status.UserLevel = (int)guidance.UserLevel;
                        UpdateStatus(status);
                    }
                }
            }
        }

        private bool ShowNotification(GuidanceStep guidance)
        {
            var url = guidance.Url;

            if (IsDevBuild)
            {
                tracer.Trace("Showing notification: {0}", this, url);
            }

            return notificationService.ShowPage(url);
        }

        private void UpdateStatus(SpecFlowInstallationStatus status)
        {
            statusAccessor.UpdateStatus(status);
        }

        private SpecFlowInstallationStatus GetInstallStatus()
        {
            return statusAccessor.GetInstallStatus();
        }
    }
}
