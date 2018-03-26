using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
        }

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        static int Main(string[] args)
        {
            if (args.Contains("--debug"))
            {
                Debugger.Launch();
            }

            var assemblies = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            foreach (string assembly in assemblies)
            {
                Assembly.LoadFile(assembly);
            }


            return CommandLine.Parser.Default.ParseArguments<GetTestFullPathParameters, DetectGeneratedTestVersionParameters, GenerateTestFileParameters>(args)
                .MapResult(
                    (GetTestFullPathParameters opts) => new GetTestFullPathAction().GetTestFullPath(opts),
                    (DetectGeneratedTestVersionParameters opts) => new DetectGeneratedTestVersionAction().DetectGeneratedTestVersion(opts),
                    (GenerateTestFileParameters opts) => new GenerateTestFileAction().GenerateTestFile(opts),
                    errs => 1);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
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
