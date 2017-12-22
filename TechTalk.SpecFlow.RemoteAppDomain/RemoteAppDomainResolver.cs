using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TechTalk.SpecFlow.RemoteAppDomain
{
    [Serializable]
    public class RemoteAppDomainResolver : MarshalByRefObject, IDisposable
    {
        private Info _info;
        private const string LogCategory = "RemoteAppDomainTestGeneratorFactory";

        public RemoteAppDomainResolver()
        {
            _info = new Info();
        }
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
        }

        public void Init(Info info)
        {
            _info = info;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }


        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("CurrentDomainOnAssemblyResolve");
            Debug.WriteLine("CurrentDomainOnAssemblyResolve");
            Debug.WriteLine(String.Format("GeneratorAssemlbyResolveEvent: Name: {0}; ", args.Name), LogCategory);
            Debug.WriteLine(
                String.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Fullname: {0}",
                    (args != null ? args.RequestingAssembly : null) != null ? args.RequestingAssembly.FullName : null), LogCategory);
            Debug.WriteLine(
                String.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Location: {0}",
                    args != null ? (args.RequestingAssembly != null ? args.RequestingAssembly.Location : null) : null), LogCategory);


            var assemblyName = args.Name.Split(new[] {','}, 2)[0];
            if (assemblyName.Equals(_info.RemoteGeneratorAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var generatorAssemblyPath = Path.Combine(_info.GeneratorFolder, _info.RemoteGeneratorAssemblyName + ".dll");
                Debug.WriteLine(String.Format("generatorAssemblyPath: {0}", generatorAssemblyPath), LogCategory);
                var generatorAssembly = Assembly.LoadFile(generatorAssemblyPath);

                return generatorAssembly;
            }
            if (assemblyName.Equals(_info.RemoteRuntimeAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var runtimeAssemblyPath = Path.Combine(_info.GeneratorFolder, _info.RemoteRuntimeAssemblyName + ".dll");
                Debug.WriteLine(String.Format("runtimeAssemblyPath: {0}", runtimeAssemblyPath), LogCategory);
                var runtimeAssembly = Assembly.LoadFile(runtimeAssemblyPath);
                return runtimeAssembly;
            }
            return null;
        }

        public void Init(string infoGeneratorFolder)
        {
            _info.GeneratorFolder = infoGeneratorFolder;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

        }
    }
}