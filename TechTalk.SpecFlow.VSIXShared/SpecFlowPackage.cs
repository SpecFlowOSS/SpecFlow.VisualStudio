using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using BoDi;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Utils;
using TechTalk.SpecFlow.VsIntegration.Implementation;
using TechTalk.SpecFlow.VsIntegration.Implementation.Commands;
using TechTalk.SpecFlow.VsIntegration.Implementation.Notifications;
using TechTalk.SpecFlow.VsIntegration.Implementation.Options;
using TechTalk.SpecFlow.VsIntegration.Options;
using Task = System.Threading.Tasks.Task;

namespace TechTalk.SpecFlow.VsIntegration
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the information needed to show that this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", GuidList.ProductId, IconResourceID = 400)]
    [ProvideOptionPage(typeof(OptionsPageGeneral), IntegrationOptionsProvider.SPECFLOW_OPTIONS_CATEGORY, IntegrationOptionsProvider.SPECFLOW_GENERAL_OPTIONS_PAGE, 121, 122, true)]
    [ProvideProfile(typeof(OptionsPageGeneral), IntegrationOptionsProvider.SPECFLOW_OPTIONS_CATEGORY, IntegrationOptionsProvider.SPECFLOW_GENERAL_OPTIONS_PAGE, 121, 123, true, DescriptionResourceID = 121)]
    [Guid(GuidList.guidSpecFlowPkgString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class SpecFlowPackagePackage : AsyncPackage
    {
        public IObjectContainer Container { get; private set; }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SpecFlowPackagePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.GetName().Name.StartsWith("TechTalk.SpecFlow") && args.LoadedAssembly.Location.Contains("\\bin\\Debug"))
            {
                Debugger.Break();
            }
        }

        public static IdeIntegration.Install.IdeIntegration? CurrentIdeIntegration
        {
            get
            {
                switch(VSVersion.FullVersion.Major)
                {
                    case 12:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2013;
                    case 14:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2015;
                    case 15:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2017;
                    case 16:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2019;
                    case 17:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2022;
                }
                return IdeIntegration.Install.IdeIntegration.Unknown;
            }
        }

        public static string AssemblyName
        {
            get
            {
                switch (CurrentIdeIntegration)
                {
                    case IdeIntegration.Install.IdeIntegration.VisualStudio2013:
                        return "TechTalk.SpecFlow.VsIntegration.2013";
                    case IdeIntegration.Install.IdeIntegration.VisualStudio2015:
                        return "TechTalk.SpecFlow.VsIntegration.2015";
                    case IdeIntegration.Install.IdeIntegration.VisualStudio2017:
                        return "TechTalk.SpecFlow.VsIntegration.2017";
                    case IdeIntegration.Install.IdeIntegration.VisualStudio2019:
                        return "TechTalk.SpecFlow.VisualStudioIntegration";
                    case IdeIntegration.Install.IdeIntegration.VisualStudio2022:
                        return "TechTalk.SpecFlow.VsIntegration.2022";
                    default:
                        return "TechTalk.SpecFlow.VsIntegration";
                }
            }
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            Container = VsContainerBuilder.CreateContainer(this);
            TelemetryConfiguration.Active.InstrumentationKey = AppInsightsInstrumentationKey.Key;

            var currentIdeIntegration = CurrentIdeIntegration;
            if (currentIdeIntegration != null)
            {
                InstallServices installServices = Container.Resolve<InstallServices>();
                installServices.OnPackageLoad(currentIdeIntegration.Value);
            }

            OleMenuCommandService menuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                foreach (var menuCommandHandler in Container.Resolve<IDictionary<SpecFlowCmdSet, MenuCommandHandler>>())
                {
                    menuCommandHandler.Value.RegisterTo(menuCommandService, menuCommandHandler.Key);
                }
            }

            var notificationService = Container.Resolve<NotificationService>();
            await notificationService.InitializeAsync(cancellationToken);

            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}
