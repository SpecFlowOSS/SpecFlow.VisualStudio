using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using BoDi;
using EnvDTE;
using EnvDTE80;
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Analytics;
using TechTalk.SpecFlow.VsIntegration.Implementation;
using TechTalk.SpecFlow.VsIntegration.Install;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.TestRunner;
using TechTalk.SpecFlow.VsIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Tracing.OutputWindow;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration
{
    public static class VsContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(SpecFlowPackagePackage package)
        {
            var container = new ObjectContainer();

            container.RegisterInstanceAs(package);
            container.RegisterInstanceAs<IServiceProvider>(package);

            RegisterDefaults(container);

            BiDiContainerProvider.Init(container); //TODO: avoid static field

            return container;
        }

        private static void RegisterDefaults(IObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }

   

   
}
