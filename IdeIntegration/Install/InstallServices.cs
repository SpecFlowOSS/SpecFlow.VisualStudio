using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class InstallServices
    {
        private const int AFTER_RAMP_UP_DAYS = 10;
        private const int EXPERIENCED_DAYS = 100;
        private const int VETERAN_DAYS = 200;

        private readonly IIdeTracer tracer;
        private readonly IGuidanceNotificationService notificationService;
        private readonly IFileAssociationDetector fileAssociationDetector;
        private readonly IStatusAccessor statusAccessor;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;

        public IdeIntegration IdeIntegration { get; private set; }
        public static Version CurrentVersion
        {
            get
            {
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return new Version(assemblyVersion.Major, assemblyVersion.Minor);
            }
        }

        internal static bool IsDevBuild
        {
            get { return CurrentVersion.Equals(new Version(1, 0)); }
        }

        public InstallServices(IGuidanceNotificationService notificationService, IIdeTracer tracer, IFileAssociationDetector fileAssociationDetector, IStatusAccessor statusAccessor, IAnalyticsTransmitter analyticsTransmitter)
        {
            this.notificationService = notificationService;
            this.tracer = tracer;
            this.fileAssociationDetector = fileAssociationDetector;
            this.statusAccessor = statusAccessor;
            _analyticsTransmitter = analyticsTransmitter;
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
                if (ShowNotification(GuidanceNotification.AfterInstall))
                {
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

            _analyticsTransmitter.TransmitLogonEvent("Microsoft Visual Studio", IdeIntegration.ToString(), CurrentVersion.ToString());
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
                if (ShowNotification(GuidanceNotification.Upgrade, isSpecRunUsed))
                {
                    status.InstallDate = today;
                    status.InstalledVersion = CurrentVersion;

                    UpdateStatus(status);
                }
            }
            else if (status.UsageDays >= AFTER_RAMP_UP_DAYS && status.UserLevel < (int)GuidanceNotification.AfterRampUp)
            {
                if (ShowNotification(GuidanceNotification.AfterRampUp, isSpecRunUsed))
                {
                    status.UserLevel = (int)GuidanceNotification.AfterRampUp;
                    UpdateStatus(status);
                }
            }
            else if (status.UsageDays >= EXPERIENCED_DAYS && status.UserLevel < (int)GuidanceNotification.Experienced)
            {
                if (ShowNotification(GuidanceNotification.Experienced, isSpecRunUsed))
                {
                    status.UserLevel = (int)GuidanceNotification.Experienced;
                    UpdateStatus(status);
                }
            }
            else if (status.UsageDays >= VETERAN_DAYS && status.UserLevel < (int)GuidanceNotification.Veteran)
            {
                if (ShowNotification(GuidanceNotification.Veteran, isSpecRunUsed))
                {
                    status.UserLevel = (int)GuidanceNotification.Veteran;
                    UpdateStatus(status);
                }
            }
        }

        private bool ShowNotification(GuidanceNotification guidanceNotification, bool isSpecRunUsed = false)
        {
            int linkid = (int)guidanceNotification + (int)IdeIntegration;
            string url = string.Format("http://go.specflow.org/g{0}{1}{2}{3}", linkid, CurrentVersion.Major, CurrentVersion.Minor, isSpecRunUsed ? "p" : "");

            if (IsDevBuild)
            {
                tracer.Trace("Showing notification: {0}", this, url);
                url += "-dev";
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
