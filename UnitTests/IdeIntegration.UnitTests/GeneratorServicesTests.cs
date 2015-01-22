using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.UnitTests
{
    internal class GeneratorServicesMock : GeneratorServices
    {
        private readonly Func<ProjectSettings> getProjectSettings;

        public GeneratorServicesMock(ITestGeneratorFactory testGeneratorFactory, bool enableSettingsCache, Func<ProjectSettings> getProjectSettings) : base(testGeneratorFactory, new Mock<IIdeTracer>().Object, enableSettingsCache)
        {
            this.getProjectSettings = getProjectSettings;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            if (getProjectSettings == null)
                return new ProjectSettings();

            return getProjectSettings();
        }
    }

    [TestFixture]
    public class GeneratorServicesTests
    {
        Mock<ITestGeneratorFactory> TestGeneratorFactoryStub;
        Mock<ITestGenerator> TestGeneratorStub;

        [SetUp]
        public void Setup()
        {
            TestGeneratorFactoryStub = new Mock<ITestGeneratorFactory>();
            TestGeneratorStub = new Mock<ITestGenerator>();

        }

        [Test]
        public void Should_create_test_generator()
        {
            var generatorServices = new GeneratorServicesMock(TestGeneratorFactoryStub.Object, false, null);
            TestGeneratorFactoryStub.Setup(tgf => tgf.CreateGenerator(It.IsAny<ProjectSettings>())).Returns(TestGeneratorStub.Object);

            var result = generatorServices.CreateTestGenerator();

            result.Should().NotBeNull();
            result.Should().Be(TestGeneratorStub.Object);
        }

        [Test]
        public void Should_not_cache_project_settings_when_not_enabled()
        {
            int settingsCounter = 0;

            var generatorServices = new GeneratorServicesMock(TestGeneratorFactoryStub.Object, false,
                () => { settingsCounter++; return new ProjectSettings();});
            TestGeneratorFactoryStub.Setup(tgf => tgf.CreateGenerator(It.IsAny<ProjectSettings>())).Returns(TestGeneratorStub.Object);

            generatorServices.CreateTestGenerator();
            generatorServices.CreateTestGenerator();

            settingsCounter.Should().Be(2);
        }

        [Test]
        public void Should_requery_project_settings_when_invalidated()
        {
            int settingsCounter = 0;

            var generatorServices = new GeneratorServicesMock(TestGeneratorFactoryStub.Object, false,
                () => { settingsCounter++; return new ProjectSettings();});
            TestGeneratorFactoryStub.Setup(tgf => tgf.CreateGenerator(It.IsAny<ProjectSettings>())).Returns(TestGeneratorStub.Object);

            generatorServices.CreateTestGenerator();
            generatorServices.InvalidateSettings();
            generatorServices.CreateTestGenerator();

            settingsCounter.Should().Be(2);
        }

        [Test]
        public void Should_cache_project_settings_when_enabled()
        {
            int settingsCounter = 0;

            var generatorServices = new GeneratorServicesMock(TestGeneratorFactoryStub.Object, true,
                () => { settingsCounter++; return new ProjectSettings();});
            TestGeneratorFactoryStub.Setup(tgf => tgf.CreateGenerator(It.IsAny<ProjectSettings>())).Returns(TestGeneratorStub.Object);

            generatorServices.CreateTestGenerator();
            generatorServices.CreateTestGenerator();

            settingsCounter.Should().Be(1);
        }
    }
}
