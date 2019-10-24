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
        Mock<IDirectoryService> directoryServiceStub;
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

        private void GivenFileExists(bool fileExists)
        {
            fileServiceStub.Setup(fileService => fileService.Exists(It.IsAny<string>())).Returns(fileExists);
        }

        private void GivenDirectoryExists(bool directoryExists)
        {
            directoryServiceStub.Setup(directoryService => directoryService.Exists(It.IsAny<string>())).Returns(directoryExists);
        }

        private void GivenDirectoryName(string directoryName)
        {
            directoryServiceStub.Setup(directoryService => directoryService.GetDirectoryName(It.IsAny<string>())).Returns(directoryName);
        }

        [SetUp]
        public void Setup()
        {
            windowsRegistryStub = new Mock<IWindowsRegistry>();
            fileServiceStub = new Mock<IFileService>();
            directoryServiceStub = new Mock<IDirectoryService>();
            sut = new FileUserIdStore(windowsRegistryStub.Object, fileServiceStub.Object, directoryServiceStub.Object);
        }

        [Test]
        public void Should_GetUserIdFromFile_WhenFileExists()
        {
            GivenFileExists(true);
            GivenUserIdStringInFile(UserId);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.ReadAllText(FileUserIdStore.UserIdFilePath));
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_GetUserIdFromRegistry_WhenFileDoesNotExist()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(UserIdInRegistry);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_MigrateExistingUserIdFromRegistryToFile_WhenFileDoesNotExist()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(UserIdInRegistry);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, UserId), Times.Once());
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_NotMigrateNotValidUserIdFromRegistryToFile()
        {
            var notValidGuid = "not valid guid";

            GivenFileExists(false);
            GivenUserIdStringInRegistry(notValidGuid);

            string userId = sut.GetUserId();
            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());

            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, notValidGuid), Times.Never());
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, userId), Times.Once());
            fileServiceStub.VerifyNoOtherCalls();

            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();

            userId.Should().NotBe(notValidGuid);
        }

        [Test]
        public void Should_NotMigrateExistingUserIdFromRegistryToFile_WhenFileExists()
        {
            GivenFileExists(true);
            GivenUserIdStringInFile(UserId);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(FileUserIdStore.UserIdFilePath));
            fileServiceStub.Verify(fileService => fileService.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            windowsRegistryStub.VerifyNoOtherCalls();
            userId.Should().Be(UserId);
        }

        [Test]
        public void Should_GenerateNewUserId_WhenNoUserIdExists()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(null);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();
            userId.Should().NotBeEmpty();
        }

        [Test]
        public void Should_PersistNewlyGeneratedUserId_WhenNoUserIdExists()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(null);

            string userId = sut.GetUserId();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, userId), Times.Once());
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();
        }

        [Test]
        public void Should_CreateSpecFlowDirectoryBeforePersistingUserId_WhenNeeded()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(UserIdInRegistry);
            const string directoryName = @"c:\foo\bar";
            GivenDirectoryName(directoryName);
            GivenDirectoryExists(false);
            
            sut.GetUserId();

            directoryServiceStub.Verify(directoryService => directoryService.GetDirectoryName(FileUserIdStore.UserIdFilePath));
            directoryServiceStub.Verify(directoryService => directoryService.CreateDirectory(directoryName));
        }
    }
}
