using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
{
    public interface IUserUniqueIdStore
    {
        Guid Get();
    }
}
