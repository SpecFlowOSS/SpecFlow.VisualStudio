using System;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public interface IUserUniqueIdStore
    {
        Guid Get();
    }
}
