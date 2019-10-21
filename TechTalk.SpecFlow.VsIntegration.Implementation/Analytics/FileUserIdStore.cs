﻿using System;
using TechTalk.SpecFlow.VsIntegration.Implementation.Services;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class FileUserIdStore : IUserUniqueIdStore
    {
        private const string UserIdRegistryPath = @"Software\TechTalk\SpecFlow\Vsix";
        private const string UserIdRegistryValueName = @"UserUniqueId";
        private static readonly string UserIdFilePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\SpecFlow\userid");
        private readonly Lazy<Guid> _lazyUniqueUserId;
        private readonly IWindowsRegistry _windowsRegistry;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public FileUserIdStore(IWindowsRegistry windowsRegistry, IFileService fileService, IDirectoryService directoryService)
        {
            _windowsRegistry = windowsRegistry;
            _fileService = fileService;
            _directoryService = directoryService;
            _lazyUniqueUserId = new Lazy<Guid>(FetchAndPersistUserId);
        }

        public Guid Get()
        {
            return _lazyUniqueUserId.Value;
        }

        public Guid? TryFetchUserIdFromRegistry()
        {
            if (!(_windowsRegistry.GetValueForCurrentUser(UserIdRegistryPath, UserIdRegistryValueName, null) is string
                uniqueUserIdString))
            {
                return null;
            }
            return Guid.ParseExact(uniqueUserIdString, "B");
        }

        public Guid FetchAndPersistUserId()
        {
            if (_fileService.Exists(UserIdFilePath))
            {
                var userIdStringFromFile = _fileService.ReadAllText(UserIdFilePath);
                if (Guid.TryParse(userIdStringFromFile, out var userIdFromFile))
                {
                    return userIdFromFile;
                }
            }
            
            var maybeUserIdFromRegistry = TryFetchUserIdFromRegistry();
            if (maybeUserIdFromRegistry is Guid userIdFromRegistry)
            {
                PersistUserId(userIdFromRegistry);
                return userIdFromRegistry;
            }

            var generatedUserId = GenerateAndPersistUserId();
            return generatedUserId;

        }

        public void PersistUserId(Guid userId)
        {
            var userIdStringFromRegistry = userId.ToString("B");

            var directoryName = _directoryService.GetDirectoryName(UserIdFilePath);
            if (!_directoryService.Exists(directoryName))
            {
                _directoryService.CreateDirectory(directoryName);
            }

            _fileService.WriteAllText(UserIdFilePath, userIdStringFromRegistry);
        }

        public Guid GenerateAndPersistUserId()
        {
            var newUserId = Guid.NewGuid();

            PersistUserId(newUserId);

            return newUserId;
        }
    }
}
