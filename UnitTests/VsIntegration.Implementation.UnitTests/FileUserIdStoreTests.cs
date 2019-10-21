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
            const string userIdString = "{491ed5c0-9f25-4c27-941a-19b17cc81c87}";
            GivenFileExists(true);
            GivenUserIdStringInFile(userIdString);

            Guid userId = sut.Get();

            fileServiceStub.Verify(fileService => fileService.ReadAllText(FileUserIdStore.UserIdFilePath));
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            userId.ToString("B").Should().Be(userIdString);
        }

        [Test]
        public void Should_GetUserIdFromRegistry_WhenFileDoesNotExist()
        {
            const string userIdString = "{491ed5c0-9f25-4c27-941a-19b17cc81c87}";
            GivenFileExists(false);
            GivenUserIdStringInRegistry(userIdString);

            Guid userId = sut.Get();

            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            userId.ToString("B").Should().Be(userIdString);
        }

        [Test]
        public void Should_MigrateExistingUserIdFromRegistryToFile_WhenFileDoesNotExist()
        {
            const string userIdString = "{491ed5c0-9f25-4c27-941a-19b17cc81c87}";
            GivenFileExists(false);
            GivenUserIdStringInRegistry(userIdString);

            Guid userId = sut.Get();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, userIdString));
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();
            userId.ToString("B").Should().Be(userIdString);
        }

        [Test]
        public void Should_NotMigrateExistingUserIdFromRegistryToFile_WhenFileExists()
        {
            const string userIdString = "{491ed5c0-9f25-4c27-941a-19b17cc81c87}";
            GivenFileExists(true);
            GivenUserIdStringInFile(userIdString);

            Guid userId = sut.Get();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(FileUserIdStore.UserIdFilePath));
            fileServiceStub.Verify(fileService => fileService.WriteAllText(It.IsAny<string>(), userIdString), Times.Never);
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            windowsRegistryStub.VerifyNoOtherCalls();
            userId.ToString("B").Should().Be(userIdString);
        }

        [Test]
        public void Should_GenerateNewUserId_WhenNoUserIdExists()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry(null);

            Guid userId = sut.Get();

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

            Guid userId = sut.Get();

            fileServiceStub.Verify(fileService => fileService.Exists(It.IsAny<string>()));
            fileServiceStub.Verify(fileService => fileService.ReadAllText(It.IsAny<string>()), Times.Never());
            fileServiceStub.Verify(fileService => fileService.WriteAllText(FileUserIdStore.UserIdFilePath, userId.ToString("B")));
            fileServiceStub.VerifyNoOtherCalls();
            windowsRegistryStub.Verify(windowsRegistry =>
                windowsRegistry.GetValueForCurrentUser(FileUserIdStore.UserIdRegistryPath, FileUserIdStore.UserIdRegistryValueName, null));
            windowsRegistryStub.VerifyNoOtherCalls();
        }

        [Test]
        public void Should_CreateSpecFlowDirectoryBeforePersistingUserId_WhenNeeded()
        {
            GivenFileExists(false);
            GivenUserIdStringInRegistry("{491ed5c0-9f25-4c27-941a-19b17cc81c87}");
            const string directoryName = @"c:\foo\bar";
            GivenDirectoryName(directoryName);
            GivenDirectoryExists(false);
            
            sut.Get();

            directoryServiceStub.Verify(directoryService => directoryService.GetDirectoryName(FileUserIdStore.UserIdFilePath));
            directoryServiceStub.Verify(directoryService => directoryService.CreateDirectory(directoryName));
        }
    }
}
