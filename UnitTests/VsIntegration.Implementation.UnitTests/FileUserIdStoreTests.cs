using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.VsIntegration.Implementation.Analytics;
using TechTalk.SpecFlow.VsIntegration.Implementation.Services;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.UnitTests
{
    [TestFixture]
    public class FileUserIdStoreTests
    {
        private const string UserId = "491ed5c0-9f25-4c27-941a-19b17cc81c87";
        private const string UserIdInRegistry = "{491ed5c0-9f25-4c27-941a-19b17cc81c87}";
        Mock<IWindowsRegistry> windowsRegistryStub;
        Mock<IFileService> fileServiceStub;
        FileUserIdStore sut;

        private void GivenUserIdStringInRegistry(string userIdString)
        {
            windowsRegistryStub.Setup(windowsRegistry =>
                    windowsRegistry.GetValueForCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .Returns(userIdString);
        }

        private void GivenUserIdStringInFile(string userIdString)
        {
            fileServiceStub.Setup(fileService => fileService.ReadAllText(It.IsAny<string>())).Returns(userIdString);
        }

        private void GivenFileExists()
        {
            fileServiceStub.Setup(fileService => fileService.Exists(It.IsAny<string>())).Returns(true);
        }

        private void GivenFileDoesNotExists()
        {
            fileServiceStub.Setup(fileService => fileService.Exists(It.IsAny<string>())).Returns(false);
        }

        [SetUp]
        public void Setup()
        {
            windowsRegistryStub = new Mock<IWindowsRegistry>();
            fileServiceStub = new Mock<IFileService>();
            sut = new FileUserIdStore(windowsRegistryStub.Object, fileServiceStub.Object);
        }

        [Test]
        public void Should_GetUserIdFromFile_WhenFileExists()
        {
            GivenFileExists();
            GivenUserIdStringInFile(UserId);

            string userId = sut.GetUserId();
            
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_GetUserIdFromRegistry_WhenFileDoesNotExist()
        {
            GivenFileDoesNotExists();
            GivenUserIdStringInRegistry(UserIdInRegistry);

            string userId = sut.GetUserId();
            
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_MigrateExistingUserIdFromRegistryToFile_WhenFileDoesNotExist()
        {
            GivenFileDoesNotExists();
            GivenUserIdStringInRegistry(UserIdInRegistry);

            string userId = sut.GetUserId();

            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_NotMigrateNotValidUserIdFromRegistryToFile()
        {
            var notValidGuid = "not valid guid";

            GivenFileDoesNotExists();
            GivenUserIdStringInRegistry(notValidGuid);

            string userId = sut.GetUserId();

            userId.Should().NotBe(notValidGuid);
        }

        [Test]
        public void Should_NotMigrateExistingUserIdFromRegistryToFile_WhenFileExists()
        {
            GivenFileExists();
            GivenUserIdStringInFile(UserId);

            string userId = sut.GetUserId();

            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_GenerateNewUserId_WhenNoUserIdExists()
        {
            GivenFileDoesNotExists();
            GivenUserIdStringInRegistry(null);

            string userId = sut.GetUserId();

            userId.Should().NotBeEmpty();
        }
    }
}
