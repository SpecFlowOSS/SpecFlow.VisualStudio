using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Analytics;
using TechTalk.SpecFlow.VsIntegration.Install;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Tracing.OutputWindow;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    public partial class DefaultDependencyProvider
    {
        static partial void RegisterCommands(IObjectContainer container);

        public virtual void RegisterDefaults(IObjectContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();
            RegisterVsDependencies(container, serviceProvider);

            container.RegisterTypeAs<InstallServices, InstallServices>();
            container.RegisterTypeAs<InstallServicesHelper, InstallServicesHelper>();
            container.RegisterTypeAs<VsBrowserGuidanceNotificationService, IGuidanceNotificationService>();
            container.RegisterTypeAs<WindowsFileAssociationDetector, IFileAssociationDetector>();
            container.RegisterTypeAs<RegistryStatusAccessor, IStatusAccessor>();

            container.RegisterTypeAs<IntegrationOptionsProvider, IIntegrationOptionsProvider>();
            container.RegisterInstanceAs<IIdeTracer>(VsxHelper.ResolveMefDependency<IVisualStudioTracer>(serviceProvider));
            container.RegisterInstanceAs(VsxHelper.ResolveMefDependency<IProjectScopeFactory>(serviceProvider));

            container.RegisterTypeAs<StepDefinitionSkeletonProvider, IStepDefinitionSkeletonProvider>();
            container.RegisterTypeAs<DefaultSkeletonTemplateProvider, ISkeletonTemplateProvider>();
            container.RegisterTypeAs<StepTextAnalyzer, IStepTextAnalyzer>();

            container.RegisterTypeAs<TelemetryClientWrapper, TelemetryClientWrapper>();
            container.RegisterTypeAs<AppInsightsExtensionLoadedDataTransformer, IAppInsightsEventConverter<ExtensionLoadedAnalyticsEvent>>();
            container.RegisterTypeAs<AppInsightsAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();
            container.RegisterTypeAs<VisualStudioProjectTargetFrameworksProvider, IProjectTargetFrameworksProvider>();
            container.RegisterTypeAs<VisualStudioIdeInformationStore, IIdeInformationStore>();
            container.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            container.RegisterTypeAs<EnableAnalyticsChecker, IEnableAnalyticsChecker>();
            container.RegisterTypeAs<RegistryUserUniqueIdStore, IUserUniqueIdStore>();

            RegisterCommands(container);
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
        }
    }
}
