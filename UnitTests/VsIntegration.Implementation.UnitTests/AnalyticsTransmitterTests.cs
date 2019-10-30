using System;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Analytics.Events;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.VsIntegration.Implementation.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.UnitTests
{
    [TestFixture]
    public class AnalyticsTransmitterTests
    {
        Mock<IUserUniqueIdStore> userUniqueIdStoreStub;
        Mock<IEnableAnalyticsChecker> enableAnalyticsCheckerStub;
        Mock<IAnalyticsTransmitterSink> analyticsTransmitterSink;
        Mock<IIdeInformationStore> ideInformationStore;
        Mock<IProjectTargetFrameworksProvider> projectTargetFrameworksProvider;
        Mock<ICurrentExtensionVersionProvider> currentExtensionVersionProviderStub;
        AnalyticsTransmitter sut;

        [SetUp]
        public void Setup()
        {
            userUniqueIdStoreStub = new Mock<IUserUniqueIdStore>();
            enableAnalyticsCheckerStub = new Mock<IEnableAnalyticsChecker>();
            analyticsTransmitterSink = new Mock<IAnalyticsTransmitterSink>();
            ideInformationStore = new Mock<IIdeInformationStore>();
            projectTargetFrameworksProvider = new Mock<IProjectTargetFrameworksProvider>();
            currentExtensionVersionProviderStub = new Mock<ICurrentExtensionVersionProvider>();
            
            currentExtensionVersionProviderStub.Setup(ce => ce.GetCurrentExtensionVersion())
                .Returns(new Version("2019.0"));

            sut = new AnalyticsTransmitter(userUniqueIdStoreStub.Object, enableAnalyticsCheckerStub.Object, 
                analyticsTransmitterSink.Object, ideInformationStore.Object, projectTargetFrameworksProvider.Object,
                currentExtensionVersionProviderStub.Object);
        }

        private void GivenAnalyticsEnabled()
        {
            enableAnalyticsCheckerStub.Setup(analyticsChecker => analyticsChecker.IsEnabled()).Returns(true);
        }

        private void GivenAnalyticsDisabled()
        {
            enableAnalyticsCheckerStub.Setup(analyticsChecker => analyticsChecker.IsEnabled()).Returns(false);
        }

        private void GivenTheUserUniqueIdStoreThrowsUnauthorizedAccessException()
        {
            userUniqueIdStoreStub.Setup(ui => ui.GetUserId()).Throws<UnauthorizedAccessException>();
        }

        [Test]
        public void Should_NotSendAnalytics_WhenDisabled()
        {
            GivenAnalyticsDisabled();

            sut.TransmitExtensionLoadedEvent();

            enableAnalyticsCheckerStub.Verify(analyticsChecker => analyticsChecker.IsEnabled(), Times.Once);
            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Never);
        }

        [Test]
        public void Should_SendAnalytics_WhenEnabled()
        {
            GivenAnalyticsEnabled();

            sut.TransmitExtensionLoadedEvent();

            enableAnalyticsCheckerStub.Verify(analyticsChecker => analyticsChecker.IsEnabled(), Times.Once);
            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Once);
        }

        [Test]
        public void Should_TransmitExtensionLoadedEvent()
        {
            GivenAnalyticsEnabled();

            sut.TransmitExtensionLoadedEvent();
            
            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<ExtensionLoadedAnalyticsEvent>()), Times.Once);
        }

        [Test]
        public void Should_TransmitExtensionInstalledEvent()
        {
            GivenAnalyticsEnabled();

            sut.TransmitExtensionInstalledEvent();

            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<ExtensionInstalledAnalyticsEvent>()), Times.Once);
        }

        [Test]
        public void Should_TransmitExtensionUpgradedEvent()
        {
            GivenAnalyticsEnabled();

            string oldExtensionVersion = "2019.0";
            sut.TransmitExtensionUpgradedEvent(oldExtensionVersion);

            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.Is<ExtensionUpgradedAnalyticsEvent>(eu => eu.OldExtensionVersion == oldExtensionVersion)), Times.Once);
        }

        [TestCase(10, "10 day usage")]
        [TestCase(100, "100 day usage")]
        [TestCase(200, "200 day usage")]
        public void Should_TransmitExtensionUsageEvents(int daysOfUsage, string expectedEventName)
        {
            GivenAnalyticsEnabled();

            sut.TransmitExtensionUsage(daysOfUsage);

            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.Is<IAnalyticsEvent>(ajk => ajk.EventName == expectedEventName)), Times.Once);
        }

        [Test]
        public void Should_TryToSendExceptionAnalytics_WhenErrorDuringAnalyticsEventCreation()
        {
            GivenAnalyticsEnabled();
            GivenTheUserUniqueIdStoreThrowsUnauthorizedAccessException();

            sut.TransmitExtensionLoadedEvent();

            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.Is<ExceptionAnalyticsEvent>(ex => 
                ex.EventName == "System.UnauthorizedAccessException")), Times.Once);
        }

    }
}
