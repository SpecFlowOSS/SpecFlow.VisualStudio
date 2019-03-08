using System;
using Microsoft.Win32;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class RegistryUserUniqueIdStore : IUserUniqueIdStore
    {
        private const string UserUniqueIdPath = @"Software\TechTalk\SpecFlow\Vsix";
        private const string UserUniqueId = @"UserUniqueId";
        private readonly Lazy<Guid> _lazyUniqueUserId;

        public RegistryUserUniqueIdStore()
        {
            _lazyUniqueUserId = new Lazy<Guid>(FetchFromOrCreateInRegistry);
        }

        public Guid Get()
        {
            return _lazyUniqueUserId.Value;
        }

        public Guid FetchFromOrCreateInRegistry()
        {
            var rootKey = Registry.CurrentUser;
            var key = rootKey.OpenSubKey(UserUniqueIdPath);
            if (key == null)
            {
                return CreateUniqueUserIdInRegistry();
            }

            using (key)
            {
                var value = key.GetValue(UserUniqueId, null) as Guid?;
                return value ?? CreateUniqueUserIdInRegistry();
            }
        }

        public Guid CreateUniqueUserIdInRegistry()
        {
            var rootKey = Registry.CurrentUser;
            using (var key = rootKey.CreateSubKey(UserUniqueIdPath))
            {
                if (key == null)
                {
                    throw new InvalidOperationException("Could not create registry key.");
                }

                var newUserId = Guid.NewGuid();
                key.SetValue(UserUniqueId, newUserId, RegistryValueKind.Binary);

                return newUserId;
            }
        }
    }
}
