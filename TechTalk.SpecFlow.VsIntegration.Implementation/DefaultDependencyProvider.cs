using System;
using BoDi;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Analytics;
using TechTalk.SpecFlow.VsIntegration.Implementation.Commands;
using TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands;
using TechTalk.SpecFlow.VsIntegration.Implementation.Install;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Implementation.Services;
using TechTalk.SpecFlow.VsIntegration.Implementation.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Tracing.OutputWindow;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public class DefaultDependencyProvider
    {
        
        public virtual void RegisterDefaults(IObjectContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();
            RegisterVsDependencies(container, serviceProvider);
            RegisterDependencies(container);

            container.RegisterInstanceAs<IIdeTracer>(VsxHelper.ResolveMefDependency<IVisualStudioTracer>(serviceProvider));
            container.RegisterInstanceAs(VsxHelper.ResolveMefDependency<IProjectScopeFactory>(serviceProvider));

            RegisterCommands(container);
        }

        public virtual void RegisterDependencies(IObjectContainer container)
        {
            container.RegisterTypeAs<InstallServices, InstallServices>();
            container.RegisterTypeAs<InstallServicesHelper, InstallServicesHelper>();
            container.RegisterTypeAs<ExternalBrowserGuidanceNotificationService, IGuidanceNotificationService>();
            container.RegisterTypeAs<WindowsFileAssociationDetector, IFileAssociationDetector>();
            container.RegisterTypeAs<RegistryStatusAccessor, IStatusAccessor>();

            container.RegisterTypeAs<WindowsRegistry, IWindowsRegistry>();
            container.RegisterTypeAs<FileService, IFileService>();
            container.RegisterTypeAs<DirectoryService, IDirectoryService>();

            container.RegisterTypeAs<StepDefinitionSkeletonProvider, IStepDefinitionSkeletonProvider>();
            container.RegisterTypeAs<DefaultSkeletonTemplateProvider, ISkeletonTemplateProvider>();
            container.RegisterTypeAs<StepTextAnalyzer, IStepTextAnalyzer>();
            container.RegisterTypeAs<StepNameReplacer, IStepNameReplacer>();

            container.RegisterTypeAs<TelemetryClientWrapper, TelemetryClientWrapper>();
            container.RegisterTypeAs<AppInsightsEventConverter, IAppInsightsEventConverter>();
            container.RegisterTypeAs<AppInsightsAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();
            container.RegisterTypeAs<VisualStudioProjectTargetFrameworksProvider, IProjectTargetFrameworksProvider>();
            container.RegisterTypeAs<VisualStudioIdeInformationStore, IIdeInformationStore>();
            container.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            container.RegisterTypeAs<EnableAnalyticsChecker, IEnableAnalyticsChecker>();
            container.RegisterTypeAs<FileUserIdStore, IUserUniqueIdStore>();
            container.RegisterTypeAs<EnvironmentSpecFlowTelemetryChecker, IEnvironmentSpecFlowTelemetryChecker>();
            container.RegisterTypeAs<CurrentExtensionVersionProvider, ICurrentExtensionVersionProvider>();
            container.RegisterTypeAs<DevBuildChecker, IDevBuildChecker>();
        }

        protected virtual void RegisterVsDependencies(IObjectContainer container, IServiceProvider serviceProvider)
        {
            var dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null)
            {
                container.RegisterInstanceAs(dte);
                container.RegisterInstanceAs((DTE2)dte);
            }

            container.RegisterInstanceAs(VsxHelper.ResolveMefDependency<IOutputWindowService>(serviceProvider));
            container.RegisterInstanceAs(VsxHelper.ResolveMefDependency<IGherkinLanguageServiceFactory>(serviceProvider));
            container.RegisterInstanceAs(VsxHelper.ResolveMefDependency<IIntegrationOptionsProvider>(serviceProvider));
            
        }

        private void RegisterCommands(IObjectContainer container)
        {
            container.RegisterTypeAs<ReGenerateAllCommand, MenuCommandHandler>(SpecFlowCmdSet.ReGenerateAll.ToString());
            container.RegisterTypeAs<ContextDependentNavigationCommand, MenuCommandHandler>(SpecFlowCmdSet.ContextDependentNavigation.ToString());
            container.RegisterTypeAs<GenerateStepDefinitionSkeletonCommand, MenuCommandHandler>(SpecFlowCmdSet.GenerateStepDefinitionSkeleton.ToString());

            // internal commands
            container.RegisterTypeAs<GoToStepsCommand, IGoToStepsCommand>();
            container.RegisterTypeAs<GoToStepDefinitionCommand, IGoToStepDefinitionCommand>();
        }
    }
}
