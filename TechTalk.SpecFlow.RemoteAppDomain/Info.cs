using System;

namespace TechTalk.SpecFlow.RemoteAppDomain
{
    [Serializable]
    public class Info
    {
        public string GeneratorFolder { get; set; }

        public Info()
        {
            RemoteGeneratorAssemblyName = "TechTalk.SpecFlow.Generator";
            RemoteRuntimeAssemblyName = "TechTalk.SpecFlow";
        }

        public string RemoteGeneratorAssemblyName { get; private set; }
        public string RemoteRuntimeAssemblyName { get; private set; }
    }
}