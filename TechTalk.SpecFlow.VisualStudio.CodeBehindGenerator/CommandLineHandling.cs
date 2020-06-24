using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Actions;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    [Serializable]
    class CommandLineHandling : MarshalByRefObject
    {
        public CommandLineHandling()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }


        public void PreLoadAssemblies()
        {
            var assemblies = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            foreach (string assembly in assemblies)
            {
                try
                {
                    Assembly.LoadFile(assembly);
                }
                catch (Exception e)
                {
                
                }
            }
        }

        public int HandleCommandLine(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<GetTestFullPathParameters, DetectGeneratedTestVersionParameters, GenerateTestFileParameters, GetGeneratorVersionParameters>(args)
                .MapResult(
                    (GetTestFullPathParameters opts) => new GetTestFullPathAction().GetTestFullPath(opts),
                    (DetectGeneratedTestVersionParameters opts) => new DetectGeneratedTestVersionAction().DetectGeneratedTestVersion(opts),
                    (GenerateTestFileParameters opts) => new GenerateTestFileAction().GenerateTestFile(opts),
                    //todo whydo we need the version here?
                    //(GetGeneratorVersionParameters opts) => new GetGeneratorVersionAction().GetGeneratorVersion(opts),
                    errs => 1);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyAlreadyLoaded = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName == args.Name).SingleOrDefault();
            if (assemblyAlreadyLoaded != null)
            {
                return assemblyAlreadyLoaded;
            }

            var assemblyName = args.Name.Split(new[] { ',' }, 2)[0];

            var extensionPath = Path.Combine(Environment.CurrentDirectory, assemblyName + ".dll");
            if (File.Exists(extensionPath))
            {
                return Assembly.LoadFile(extensionPath);
            }

            return null;
        }
    }
}