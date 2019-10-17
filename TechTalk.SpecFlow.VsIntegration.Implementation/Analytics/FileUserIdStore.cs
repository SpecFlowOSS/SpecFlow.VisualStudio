using Microsoft.Win32;
using System;
using System.IO;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public class FileUserIdStore : IUserUniqueIdStore
    {

        private const string UserIdRegistryPath = @"Software\TechTalk\SpecFlow\Vsix";
        private const string UserIdRegistryValueName = @"UserUniqueId";
        private static readonly string UserIdFilePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\SpecFlow\userid");
        private readonly Lazy<Guid> _lazyUniqueUserId;

        public FileUserIdStore()
        {
     
            _lazyUniqueUserId = new Lazy<Guid>(FetchAndPersistUserId);
        }

        public Guid Get()
        {
            return _lazyUniqueUserId.Value;
        }

        public Guid? TryFetchUserIdFromRegistry()
        {
            var rootKey = Registry.CurrentUser;
            var key = rootKey.OpenSubKey(UserIdRegistryPath);

            using (key)
            {
                if (key == null || !(key.GetValue(UserIdRegistryValueName, null) is string uniqueUserIdString))
                {
                    return null;
                }

                return Guid.ParseExact(uniqueUserIdString, "B");
            }
        }

        public Guid FetchAndPersistUserId()
        {
            if (File.Exists(UserIdFilePath))
            {
                var userIdStringFromFile = File.ReadAllText(UserIdFilePath);
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

            var directoryName = Path.GetDirectoryName(UserIdFilePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(UserIdFilePath, userIdStringFromRegistry);
        }

        public Guid GenerateAndPersistUserId()
        {
            var newUserId = Guid.NewGuid();

            PersistUserId(newUserId);

            return newUserId;
        }
    }
}
