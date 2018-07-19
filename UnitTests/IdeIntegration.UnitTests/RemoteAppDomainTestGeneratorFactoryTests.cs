using System;
using System.IO;
using FluentAssertions;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Generator.AppDomain;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.IdeIntegration.UnitTests
{
    [TestFixture]
    public class RemoteAppDomainTestGeneratorFactoryTests
    {
        private string currentGeneratorFolder;
        private Mock<IIdeTracer> tracerStub;

        [SetUp]
        public void Setup()
        {
            currentGeneratorFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            tracerStub = new Mock<IIdeTracer>();
        }

        private RemoteAppDomainTestGeneratorFactory CreateRemoteAppDomainTestGeneratorFactory()
        {
            return CreateRemoteAppDomainTestGeneratorFactory(currentGeneratorFolder);
        }

        private RemoteAppDomainTestGeneratorFactory CreateRemoteAppDomainTestGeneratorFactory(string generatorFolder)
        {
            var factory = new RemoteAppDomainTestGeneratorFactory(tracerStub.Object);
            factory.Setup(generatorFolder);
            return factory;
        }

        [Test]
        public void Should_be_able_to_initialize()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.EnsureInitialized();
                remoteFactory.IsRunning.Should().BeTrue();
            }
        }

        [Test]
        public void Should_be_able_to_return_generator_version()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var version = remoteFactory.GetGeneratorVersion();

                version.Should().NotBeNull();
                version.Should().Be(TestGeneratorFactory.GeneratorVersion);
            }
        }

        [Test]
        public void Should_be_able_to_create_generator_with_default_config()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings());

                generator.Should().NotBeNull();
            }
        }

        [Serializable]
        private class DummyGenerator : MarshalByRefObject, ITestGenerator
        {
            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                throw new NotImplementedException();
            }

            public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public string GetTestFullPath(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                //nop;
            }

            public override string ToString()
            {
                return "DummyGenerator";
            }
        }

        [Test]
        public void Should_create_custom_generator_when_configured_so()
        {
            var configurationHolder = new SpecFlowConfigurationHolder(ConfigSource.AppConfig, string.Format(@"
                <specFlow>
                  <generator>
                  <dependencies>
                    <register type=""{0}"" as=""{1}""/>
                  </dependencies>
                  </generator>
                </specFlow>",
                typeof(DummyGenerator).AssemblyQualifiedName,
                typeof(ITestGenerator).AssemblyQualifiedName));

            var projectSettings = new ProjectSettings();
            projectSettings.ConfigurationHolder = configurationHolder;

            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(projectSettings);
                generator.ToString().Should().Be("DummyGenerator"); // since the type is wrapped, we can only check it this way
            }
        }

        [Test]
        public void Should_be_able_to_load_generator_from_another_folder()
        {
            using(var tempFolder = new TempFolder())
            {
                var runtimeAssemblyFile = typeof(BindingAttribute).Assembly.Location;
                var generatorAssemblyFile = typeof(TestGeneratorFactory).Assembly.Location;
                var utilsAssemblyFile = typeof(FileSystemHelper).Assembly.Location;
                FileSystemHelper.CopyFileToFolder(runtimeAssemblyFile, tempFolder.FolderName);
                FileSystemHelper.CopyFileToFolder(generatorAssemblyFile, tempFolder.FolderName);
                FileSystemHelper.CopyFileToFolder(utilsAssemblyFile, tempFolder.FolderName);

                using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory(tempFolder.FolderName))
                {
                    var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                    generator.Should().NotBeNull();
                }
            }
        }

        [Test]
        public void Should_cleanup_ater_dispose()
        {
            RemoteAppDomainTestGeneratorFactory remoteFactory;
            using (remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.EnsureInitialized();
            }

            remoteFactory.IsRunning.Should().BeFalse();
        }

        [Test]
        public void Should_start_running_delayed()
        {
            RemoteAppDomainTestGeneratorFactory remoteFactory;
            using (remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.IsRunning.Should().BeFalse();
            }
        }

        [Test]
        public void Should_cleanup_after_generator_disposed_when_timeout_ellapses()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.AppDomainCleanupTime = TimeSpan.FromSeconds(1);

                var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                generator.Dispose();

                Thread.Sleep(TimeSpan.FromSeconds(1.1));

                remoteFactory.IsRunning.Should().BeFalse();
            }
        }

        [Test]
        public void Should_not_cleanup_after_generator_disposed_immediately()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                generator.Dispose();

                remoteFactory.IsRunning.Should().BeTrue();
            }
        }

        [Test]
        public void Should_not_cleanup_when_one_generator_is_still_used()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator1 = remoteFactory.CreateGenerator(new ProjectSettings());
                var generator2 = remoteFactory.CreateGenerator(new ProjectSettings());
                generator1.Dispose();

                remoteFactory.IsRunning.Should().BeTrue();
            }
        }

        [Test]
        public void Should_cleanup_when_all_generators_are_disposed()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.AppDomainCleanupTime = TimeSpan.FromSeconds(1);

                var generator1 = remoteFactory.CreateGenerator(new ProjectSettings());
                var generator2 = remoteFactory.CreateGenerator(new ProjectSettings());
                generator1.Dispose();
                generator2.Dispose();

                Thread.Sleep(TimeSpan.FromSeconds(1.1));

                remoteFactory.IsRunning.Should().BeFalse();
            }
        }


        [Test]
        public void Should_be_able_generate_from_a_simple_valid_feature()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings()
                                                                  {
                                                                      ProjectFolder = Path.GetTempPath()
                                                                  });

                FeatureFileInput featureFileInput = new FeatureFileInput("Test.feature")
                                                        {
                                                            FeatureFileContent = @"
Feature: Addition

@mytag
Scenario: Add two numbers
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
"
                                                        };
                var result = generator.GenerateTestFile(featureFileInput, new GenerationSettings());

                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.GeneratedTestCode.Should().NotBeNull();
            }
        }

        [Test]
        public void Should_be_able_generate_from_a_simple_invalid_feature()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings()
                                                                  {
                                                                      ProjectFolder = Path.GetTempPath()
                                                                  });

                FeatureFileInput featureFileInput = new FeatureFileInput("Test.feature")
                                                        {
                                                            FeatureFileContent = @"
Feature: Addition
Scenario: Add two numbers
	Given I have entered 50 into the calculator
    AndXXX the keyword is misspelled
"
                                                        };
                var result = generator.GenerateTestFile(featureFileInput, new GenerationSettings());

                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.Errors.Should().NotBeNull();
                result.Errors.Should().NotBeEmpty();
            }
        }
    }
}
