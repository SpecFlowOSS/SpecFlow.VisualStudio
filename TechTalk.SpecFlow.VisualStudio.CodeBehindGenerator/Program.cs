using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Contains("--debug"))
            {
                Debugger.Launch();
            }


            var appDomainSetup = new AppDomainSetup
            {
//                ShadowCopyFiles = "true",
                ApplicationBase = Environment.CurrentDirectory,
                
            };


            var  plugincompabilityConfig = Path.Combine(Environment.CurrentDirectory, "plugincompability.config");
            if (File.Exists(plugincompabilityConfig))
            {
                appDomainSetup.ConfigurationFile = plugincompabilityConfig;
            }

            var appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);
            
            var commandLineHandlingType = typeof(CommandLineHandling);

            var commandLineHandling= (CommandLineHandling)appDomain.CreateInstanceFromAndUnwrap(commandLineHandlingType.Assembly.Location, commandLineHandlingType.FullName, true, BindingFlags.Default, null, null, null, null);

            commandLineHandling.PreLoadAssemblies();

            return commandLineHandling.HandleCommandLine(args);


        }

       
    }
}
