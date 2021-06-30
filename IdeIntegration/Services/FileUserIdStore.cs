using System;
using System.IO;

namespace TechTalk.SpecFlow.IdeIntegration.Services
{
    public class FileUserIdStore : IUserUniqueIdStore
    {
        public const string UserIdRegistryPath = @"Software\TechTalk\SpecFlow\Vsix";
        public const string UserIdRegistryValueName = @"UserUniqueId";
        public static readonly string UserIdFilePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\SpecFlow\userid");
        
        private readonly Lazy<string> _lazyUniqueUserId;
        private readonly IWindowsRegistry _windowsRegistry;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public FileUserIdStore(IWindowsRegistry windowsRegistry, IFileService fileService, IDirectoryService directoryService)
        {
            _windowsRegistry = windowsRegistry;
            _fileService = fileService;
            _directoryService = directoryService;
            _lazyUniqueUserId = new Lazy<string>(FetchAndPersistUserId);
        }

        public string GetUserId()
        {
            return _lazyUniqueUserId.Value;
        }

        private string TryFetchUserIdFromRegistry()
        {
            if ((_windowsRegistry.GetValueForCurrentUser(UserIdRegistryPath, UserIdRegistryValueName, null) 
                is string uniqueUserIdString))
            {
                if (Guid.TryParseExact(uniqueUserIdString, "B", out var parsedGuid))
                {
                    return parsedGuid.ToString();
                }
            }

            return null;
        }

        private string FetchAndPersistUserId()
        {
            if (_fileService.Exists(UserIdFilePath))
            {
                var userIdStringFromFile = _fileService.ReadAllText(UserIdFilePath);
                if (IsValidGuid(userIdStringFromFile))
                {
                    return userIdStringFromFile;
                }
            }
            
            var maybeUserIdFromRegistry = TryFetchUserIdFromRegistry();
            if (IsValidGuid(maybeUserIdFromRegistry))
            {
                PersistUserId(maybeUserIdFromRegistry);
                return maybeUserIdFromRegistry;
            }

            return GenerateAndPersistUserId();
        }

        private void PersistUserId(string userId)
        {
            var directoryName = Path.GetDirectoryName(UserIdFilePath);
            if (!_directoryService.Exists(directoryName))
            {
                _directoryService.CreateDirectory(directoryName);
            }

            _fileService.WriteAllText(UserIdFilePath, userId);
        }

        private bool IsValidGuid(string guid)
        {
            return Guid.TryParse(guid, out var parsedGuid);
        }

        private string GenerateAndPersistUserId()
        {
            var newUserId = Guid.NewGuid().ToString();

            PersistUserId(newUserId);

            return newUserId;
        }
    }
}
