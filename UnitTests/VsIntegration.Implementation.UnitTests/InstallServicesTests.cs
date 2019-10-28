using System;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.UnitTests
{
    [TestFixture]
    public class InstallServicesTests
    {
        Mock<IGuidanceNotificationService> guidanceNotificationServiceStub;
        Mock<IIdeTracer> ideTracerStub;
        Mock<IFileAssociationDetector> fileAssociationDetectorStub;
        Mock<IStatusAccessor> statusAccessorStub;
        Mock<IAnalyticsTransmitter> analyticsTransmitterStub;
        Mock<ICurrentExtensionVersionProvider> currentExtensionVersionProviderStub;
        Mock<IDevBuildChecker> devBuildCheckerStub;
        private InstallServices sut;
        
        private readonly Version _extensionVersion = new Version("2019.0");

        [SetUp]
        public void Setup()
        {
            guidanceNotificationServiceStub = new Mock<IGuidanceNotificationService>();
            ideTracerStub = new Mock<IIdeTracer>();
            fileAssociationDetectorStub = new Mock<IFileAssociationDetector>();
            statusAccessorStub = new Mock<IStatusAccessor>();
            analyticsTransmitterStub = new Mock<IAnalyticsTransmitter>();
            currentExtensionVersionProviderStub = new Mock<ICurrentExtensionVersionProvider>();
            devBuildCheckerStub = new Mock<IDevBuildChecker>();
            sut = new InstallServices(guidanceNotificationServiceStub.Object, ideTracerStub.Object, 
                fileAssociationDetectorStub.Object, statusAccessorStub.Object, analyticsTransmitterStub.Object,
                currentExtensionVersionProviderStub.Object, devBuildCheckerStub.Object);
        }

        public void GivenVisualStudioVersion()
        {
            currentExtensionVersionProviderStub.Setup(ce => ce.GetCurrentExtensionVersion())
                .Returns(_extensionVersion);
        }

        private void GivenVisualStudioExtensionIsNotInstalled()
        {
            GivenVisualStudioVersion();
            statusAccessorStub.Setup(status => status.GetInstallStatus()).Returns(new SpecFlowInstallationStatus()
            {
                InstalledVersion = null
            });
        }

        private void GivenVisualStudioExtensionIsInstalled()
        {
            GivenVisualStudioVersion();
            statusAccessorStub.Setup(status => status.GetInstallStatus()).Returns(new SpecFlowInstallationStatus()
            {
                InstalledVersion = _extensionVersion
            });
        }

        private void GivenGuidanceNotificationEnabled()
        {
            guidanceNotificationServiceStub.Setup(noti => noti.ShowPage(It.IsAny<string>())).Returns(true);
        }

        private void GivenGuidanceNotificationDisabled()
        {
            guidanceNotificationServiceStub.Setup(noti => noti.ShowPage(It.IsAny<string>())).Returns(false);
        }

        private void GivenVisualStudioExtensionInstalledAndUsed(int days, GuidanceNotification guidanceNotification)
        {
            GivenVisualStudioVersion();
            statusAccessorStub.Setup(status => status.GetInstallStatus()).Returns(new SpecFlowInstallationStatus()
            {
                InstalledVersion = _extensionVersion,
                UsageDays = days,
                UserLevel = (int)guidanceNotification
            });
        }

        public void GivenANewerVersionOfTheVisualStudioExtension()
        {
            currentExtensionVersionProviderStub.Setup(ce => ce.GetCurrentExtensionVersion())
                .Returns(new Version("2019.100"));
        }

        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2015)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2017)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2019)]
        public void Should_NotFireEvents_WhenShowGuidanceNotificationIsFalse(IdeIntegration.Install.IdeIntegration ideIntegration)
        {
            GivenGuidanceNotificationDisabled();
            GivenVisualStudioExtensionIsNotInstalled();

            sut.OnPackageLoad(ideIntegration);

            analyticsTransmitterStub.Verify(at => at.TransmitExtensionIstallatedEvent(), Times.Never);
        }

        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2015)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2017)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2019)]
        public void Should_FireExtensionInstalledEvent(IdeIntegration.Install.IdeIntegration ideIntegration)
        {
            GivenGuidanceNotificationEnabled();
            GivenVisualStudioExtensionIsNotInstalled();

            sut.OnPackageLoad(ideIntegration);

            analyticsTransmitterStub.Verify(at => at.TransmitExtensionIstallatedEvent(), Times.Once);
            analyticsTransmitterStub.Verify(at => at.TransmitExtensionLoadedEvent(), Times.Once);
        }
        
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2015)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2017)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2019)]
        public void Should_NotFireExtensionInstalledEvent_WhenExtensionInstalled(IdeIntegration.Install.IdeIntegration ideIntegration)
        {
            GivenGuidanceNotificationEnabled();
            GivenVisualStudioExtensionIsInstalled();

            sut.OnPackageLoad(ideIntegration);

            analyticsTransmitterStub.Verify(at => at.TransmitExtensionIstallatedEvent(), Times.Never);
        }

        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2015)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2017)]
        [TestCase(IdeIntegration.Install.IdeIntegration.VisualStudio2019)]
        public void Should_FireExtensionLoadedEvent(IdeIntegration.Install.IdeIntegration ideIntegration)
        {
            GivenGuidanceNotificationEnabled();
            GivenVisualStudioExtensionIsInstalled();

            sut.OnPackageLoad(ideIntegration);

            analyticsTransmitterStub.Verify(at => at.TransmitExtensionLoadedEvent(), Times.Once);
        }

        [Test]
        public void Should_FireExtensionUpgradedEvent()
        {
            GivenGuidanceNotificationEnabled();
            GivenVisualStudioExtensionIsInstalled();
            GivenANewerVersionOfTheVisualStudioExtension();

            sut.OnPackageUsed(true);

            analyticsTransmitterStub.Verify(at => at.TransmitExtensionUpgradedEvent(_extensionVersion.ToString()), Times.Once);
        }

        [TestCase(10, GuidanceNotification.AfterInstall, 1)]
        [TestCase(100, GuidanceNotification.AfterRampUp, 1)]
        [TestCase(200, GuidanceNotification.Experienced, 1)]
        [TestCase(1, GuidanceNotification.AfterRampUp, 0)]
        [TestCase(99, GuidanceNotification.AfterRampUp, 0)]
        [TestCase(500, GuidanceNotification.Veteran, 0)]
        public void Should_FireExtensionUsageEvents(int daysOfUsage, GuidanceNotification notificationLevel, int callCount)
        {
            GivenGuidanceNotificationEnabled();
            GivenVisualStudioExtensionInstalledAndUsed(daysOfUsage, notificationLevel);

            sut.OnPackageUsed(true);
            
            analyticsTransmitterStub.Verify(at => at.TransmitExtensionUsage(daysOfUsage), Times.Exactly(callCount));
        }

    }
}
