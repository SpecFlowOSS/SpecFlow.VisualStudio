using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Implementation.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.UnitTests
{
    [TestFixture]
    public class EnableAnalyticsCheckerTests
    {
        private Mock<IEnvironmentSpecFlowTelemetryChecker> environmentSpecFlowTelemetryCheckerStub;
        private Mock<IIntegrationOptionsProvider> integrationOptionsProviderStub;
        EnableAnalyticsChecker sut;

        [SetUp]
        public void Setup()
        {
            integrationOptionsProviderStub = new Mock<IIntegrationOptionsProvider>();
            environmentSpecFlowTelemetryCheckerStub = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            sut = new EnableAnalyticsChecker(integrationOptionsProviderStub.Object, environmentSpecFlowTelemetryCheckerStub.Object);
        }

        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, true, false)]
        public void Should_SendAnalytics_BasedOnOptionsAndEnvironment(bool visualStudioAnalyticsOption, bool environmentEnabled, bool expectedAnalyticsEnabled)
        {
            integrationOptionsProviderStub.Setup(st => st.GetOptions()).Returns(new IntegrationOptions()
            {
                OptOutDataCollection = visualStudioAnalyticsOption
            });
            environmentSpecFlowTelemetryCheckerStub.Setup(st => st.IsSpecFlowTelemetryEnabled()).Returns(environmentEnabled);

            bool analyticsEnabled = sut.IsEnabled();

            analyticsEnabled.Should().Be(expectedAnalyticsEnabled);
        }
    }
}
