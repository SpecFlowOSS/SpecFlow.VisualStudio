using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.IdeIntegration.Configuration;
using TechTalk.SpecFlow.VsIntegration.Generator;
using TechTalk.SpecFlow.VsIntegration.GherkinFileEditor;
using TechTalk.SpecFlow.VsIntegration.StepSuggestions;
using TechTalk.SpecFlow.VsIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public class VsProjectScope : IProjectScope
    {
        private const string VsProjectScopeTraceCategory = "VsProjectScope";

        private readonly Project _project;
        private readonly DteWithEvents _dteWithEvents;
        private readonly IVisualStudioTracer _tracer;
        private readonly IIntegrationOptionsProvider _integrationOptionsProvider;
        private readonly GherkinTextBufferParser _parser;
        private readonly GherkinScopeAnalyzer _analyzer = null;
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public IGeneratorServices GeneratorServices { get; private set; }

        private bool _initialized;
        private bool _initializing;
        
        // delay initialized members
        private SpecFlowConfiguration _specFlowConfiguration;
        private GherkinDialectServices _gherkinDialectServices;
        private VsProjectFileTracker _appConfigTracker;
        private ProjectFeatureFilesTracker _featureFilesTracker;
        private BindingFilesTracker _bindingFilesTracker;
        private VsStepSuggestionProvider _stepSuggestionProvider;
        private IStepDefinitionMatchService _stepDefinitionMatchService;

        public SpecFlowConfiguration SpecFlowConfiguration
        {
            get
            {
                EnsureInitialized();
                return _specFlowConfiguration;
            }
        }

        public GherkinDialectServices GherkinDialectServices
        {
            get
            {
                EnsureInitialized();
                return _gherkinDialectServices;
            }
        }

        internal ProjectFeatureFilesTracker FeatureFilesTracker
        {
            get
            {
                EnsureInitialized();
                return _featureFilesTracker;
            }
        }

        internal BindingFilesTracker BindingFilesTracker
        {
            get
            {
                EnsureInitialized();
                return _bindingFilesTracker;
            }
        }

        public VsStepSuggestionProvider StepSuggestionProvider
        {
            get
            {
                EnsureInitialized();
                return _stepSuggestionProvider;
            }
        }

        public IStepDefinitionMatchService BindingMatchService
        {
            get
            {
                EnsureInitialized();
                return _stepDefinitionMatchService;
            }
        }

        public Project Project { get { return _project; } }
        public IIdeTracer Tracer { get { return _tracer; } }
        internal DteWithEvents DteWithEvents { get { return _dteWithEvents; } }

        public IIntegrationOptionsProvider IntegrationOptionsProvider
        {
            get { return _integrationOptionsProvider; }
        }

        public event Action SpecFlowConfigurationChanged;
        public event Action GherkinDialectServicesChanged;

        internal VsProjectScope(Project project, DteWithEvents dteWithEvents, GherkinFileEditorClassifications classifications, IVisualStudioTracer tracer, IIntegrationOptionsProvider integrationOptionsProvider)
        {
            Classifications = classifications;
            _project = project;
            _dteWithEvents = dteWithEvents;
            _tracer = tracer;
            _integrationOptionsProvider = integrationOptionsProvider;

            var integrationOptions = integrationOptionsProvider.GetOptions();

            _parser = new GherkinTextBufferParser(this, tracer);
//TODO: enable when analizer is implemented
//            if (integrationOptions.EnableAnalysis)
//                analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);

            GherkinProcessingScheduler = new GherkinProcessingScheduler(tracer, integrationOptions.EnableAnalysis);

            GeneratorServices = new VsGeneratorServices(project, new VsSpecFlowConfigurationReader(project, tracer), tracer, integrationOptionsProvider);
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                lock(this)
                {
                    if (!_initialized)
                    {
                        if (_initializing)
                        {
                            _tracer.Trace("ERROR: Nested VsProjectScope is triggered by the initialize. This is bad. Please record the following stack trace: {1}{0}", this, Environment.StackTrace, Environment.NewLine);
                            return;
                        }

                        try
                        {
                            _initializing = true;
                            Initialize();
                        }
                        finally
                        {
                            _initializing = false;
                        }
                    }
                }
            }
        }

        private class StepDefinitionMatchServiceWithOnlySimpleTypeConverter : StepDefinitionMatchService
        {
            public StepDefinitionMatchServiceWithOnlySimpleTypeConverter(IBindingRegistry bindingRegistry) : base(bindingRegistry, new OnlySimpleConverter())
            {
            }

            protected override IEnumerable<BindingMatch> GetCandidatingBindingsForBestMatch(StepInstance stepInstance, CultureInfo bindingCulture)
            {
                var normalResult = base.GetCandidatingBindingsForBestMatch(stepInstance, bindingCulture).ToList();
                if (normalResult.Count > 0)
                    return normalResult;

                return GetCandidatingBindings(stepInstance, bindingCulture, useParamMatching: false); // we disable param checking
            }
        }

        private class OnlySimpleConverter : IStepArgumentTypeConverter
        {
            public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
            {
                throw new NotSupportedException();
            }

            [DebuggerStepThrough]
            public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
            {
                if (!(typeToConvertTo is RuntimeBindingType))
                {
                    try
                    {
                        // in some special cases, Type.GetType throws exception
                        // one of such case, if a Dictionary<string,string> step parameter is specified, see issue #340
                        Type systemType = Type.GetType(typeToConvertTo.FullName, false);
                        if (systemType == null)
                            return false;
                        typeToConvertTo = new RuntimeBindingType(systemType);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return false;
                    }
                }

                return StepArgumentTypeConverter.CanConvertSimple(typeToConvertTo, value, cultureInfo);
            }
        }


        private void Initialize()
        {
            _tracer.Trace("Initializing...", VsProjectScopeTraceCategory);
            try
            {
                _specFlowConfiguration = LoadSpecFlowConfiguration();
                _gherkinDialectServices = new GherkinDialectServices(_specFlowConfiguration.FeatureLanguage);

                //todo: tracker for json?
                _appConfigTracker = new VsProjectFileTracker(_project, "App.config", _dteWithEvents, _tracer);
                _appConfigTracker.FileChanged += AppConfigTrackerOnFileChanged;
                _appConfigTracker.FileOutOfScope += AppConfigTrackerOnFileOutOfScope;

                bool enableAnalysis = _integrationOptionsProvider.GetOptions().EnableAnalysis;
                if (enableAnalysis)
                {
                    _featureFilesTracker = new ProjectFeatureFilesTracker(this);
                    _featureFilesTracker.Ready += FeatureFilesTrackerOnReady;

                    _bindingFilesTracker = new BindingFilesTracker(this);

                    _stepSuggestionProvider = new VsStepSuggestionProvider(this);
                    _stepSuggestionProvider.Ready += StepSuggestionProviderOnReady;
                    _stepDefinitionMatchService = new StepDefinitionMatchServiceWithOnlySimpleTypeConverter(_stepSuggestionProvider);
                }
                _tracer.Trace("Initialized", VsProjectScopeTraceCategory);
                _initialized = true;

                if (enableAnalysis)
                {
                    _tracer.Trace("Starting analysis services...", VsProjectScopeTraceCategory);

                    _stepSuggestionProvider.Initialize();
                    _bindingFilesTracker.Initialize();
                    _featureFilesTracker.Initialize();

                    LoadStepMap();

                    _bindingFilesTracker.Run();
                    _featureFilesTracker.Run();

                    _dteWithEvents.OnBuildDone += BuildEventsOnOnBuildDone;

                    _tracer.Trace("Analysis services started", VsProjectScopeTraceCategory);
                }
                else
                {
                    _tracer.Trace("Analysis services disabled", VsProjectScopeTraceCategory);
                }
            }
            catch(Exception exception)
            {
                _tracer.Trace("Exception: " + exception, VsProjectScopeTraceCategory);
            }
        }

        private void FeatureFilesTrackerOnReady()
        {
            //compare generated file versions with the generator version
            var generatorVersion = GeneratorServices.GetGeneratorVersion(); //TODO: cache GeneratorVersion
            if (generatorVersion == null)
                return;

            Func<FeatureFileInfo, bool> outOfDateFiles = ffi => ffi.GeneratorVersion != null && ffi.GeneratorVersion < generatorVersion;
            if (_featureFilesTracker.Files.Any(outOfDateFiles))
            {
                var questionResult = MessageBox.Show(
                    @"SpecFlow detected that some of the feature files were generated with an earlier version of SpecFlow. Do you want to re-generate them now?",
                    @"SpecFlow Generator Version Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (questionResult != DialogResult.Yes)
                {
                    return;
                }

                _featureFilesTracker.ReGenerateAll(outOfDateFiles);
            }
        }

        private void ConfirmReGenerateFilesOnConfigChange()
        {
            if (_integrationOptionsProvider.GetOptions().DisableRegenerateFeatureFilePopupOnConfigChange)
            {
                return;
            }

            var questionResult = MessageBox.Show(
                @"SpecFlow detected changes in the configuration that might require re-generating the feature files. You can disable this popup in the SpecFlow Visual Studio settings (""Tools / Options / SpecFlow"")." + Environment.NewLine + @"Do you want to re-generate them now?", 
                @"SpecFlow Configuration Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (questionResult != DialogResult.Yes)
            {
                return;
            }

            _featureFilesTracker.ReGenerateAll();
        }

        private void AppConfigTrackerOnFileChanged(ProjectItem appConfigItem)
        {
            var newConfig = LoadSpecFlowConfiguration();
            if (newConfig.Equals(SpecFlowConfiguration))
            {
                return;
            }

            bool dialectServicesChanged = !newConfig.FeatureLanguage.Equals(GherkinDialectServices.DefaultLanguage);

            _specFlowConfiguration = newConfig;
            OnSpecFlowConfigurationChanged();

            if (dialectServicesChanged)
            {
                _gherkinDialectServices = new GherkinDialectServices(SpecFlowConfiguration.FeatureLanguage);
                OnGherkinDialectServicesChanged();
            }
        }

        private void AppConfigTrackerOnFileOutOfScope(ProjectItem projectItem, string projectRelativeFileName)
        {
            AppConfigTrackerOnFileChanged(projectItem);                
        }
        
        private SpecFlowConfiguration LoadSpecFlowConfiguration()
        {
            var defaultSpecFlowConfiguration = ConfigurationLoader.GetDefault();
            try
            {
                var reader = new VsSpecFlowConfigurationReader(_project, _tracer);
                var specflowLoader = new ConfigurationLoader();

                return specflowLoader.Load(defaultSpecFlowConfiguration, reader.ReadConfiguration());
            }
            catch (Exception ex)
            {
                _tracer.Trace("Configuration loading error: " + ex, VsProjectScopeTraceCategory);
                return defaultSpecFlowConfiguration;
            }
        }

        private void OnSpecFlowConfigurationChanged()
        {
            _tracer.Trace("SpecFlow configuration changed", VsProjectScopeTraceCategory);
            if (SpecFlowConfigurationChanged != null)
                SpecFlowConfigurationChanged();

            GeneratorServices.InvalidateSettings();

            ConfirmReGenerateFilesOnConfigChange();
        }

        private void OnGherkinDialectServicesChanged()
        {
            _tracer.Trace("default language changed", VsProjectScopeTraceCategory);
            if (GherkinDialectServicesChanged != null)
                GherkinDialectServicesChanged();
        }

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return _parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get
            {
                return _analyzer;
            }
        }

        private void StepSuggestionProviderOnReady()
        {
            SaveStepMap();
        }

        public void Dispose()
        {
            _dteWithEvents.OnBuildDone -= BuildEventsOnOnBuildDone;
            SaveStepMap();

            GherkinProcessingScheduler.Dispose();
            if (_appConfigTracker != null)
            {
                _appConfigTracker.FileChanged -= AppConfigTrackerOnFileChanged;
                _appConfigTracker.FileOutOfScope -= AppConfigTrackerOnFileOutOfScope;
                _appConfigTracker.Dispose();
            }
            if (_stepSuggestionProvider != null)
            {
                _stepSuggestionProvider.Dispose();
            }
            if (_featureFilesTracker != null)
            {
                _featureFilesTracker.Ready -= FeatureFilesTrackerOnReady;
                _featureFilesTracker.Dispose();
            }
            if (_bindingFilesTracker != null)
            {
                _bindingFilesTracker.Dispose();
            }
        }

        private string _stepMapFileName;

        private string GetStepMapFileName()
        {
            return _stepMapFileName ?? (_stepMapFileName = Path.Combine(Path.GetTempPath(), 
                string.Format(@"specflow-stepmap-{1}-{2}-{0}{3}.cache", VsxHelper.GetProjectUniqueId(_project), _project.Name, Math.Abs(VsxHelper.GetProjectFolder(_project).GetHashCode()), GetConfigurationText())));
        }

        private string GetConfigurationText()
        {
            //TODO: once we can better track config changes, we can also have different cache for the different configs
#if USE_CONFIG_DEPENDENT_CACHE
            try
            {
                return "-" + _project.ConfigurationManager.ActiveConfiguration.ConfigurationName + "-" +
                       _project.ConfigurationManager.ActiveConfiguration.PlatformName;
            }
            catch(Exception ex)
            {
                _tracer.Trace("Unable to get configuration name: " + ex, GetType().Name);
                return "-na";
            }
#else
            return string.Empty;
#endif
        }

        private void BuildEventsOnOnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            SaveStepMap();
        }

        private void SaveStepMap()
        {
            if (_featureFilesTracker == null || !_featureFilesTracker.IsInitialized || _bindingFilesTracker == null || !_bindingFilesTracker.IsInitialized)
            {
                return;
            }

            if (!_featureFilesTracker.IsStepMapDirty && !_bindingFilesTracker.IsStepMapDirty)
            {
                _tracer.Trace("Step map up-to-date", typeof(StepMap).Name);
                return;
            }

            var stepMap = StepMap.CreateStepMap(GherkinDialectServices.DefaultLanguage);
            _featureFilesTracker.SaveToStepMap(stepMap);
            _bindingFilesTracker.SaveToStepMap(stepMap);

            stepMap.SaveToFile(GetStepMapFileName(), _tracer);
        }

        private void LoadStepMap()
        {
            string fileName = GetStepMapFileName();
            if (!File.Exists(fileName))
            {
                return;
            }

            var stepMap = StepMap.LoadFromFile(fileName, _tracer);
            if (stepMap != null)
            {
                if (stepMap.DefaultLanguage.Equals(GherkinDialectServices.DefaultLanguage)) // if default language changed in config => ignore cache
                    _featureFilesTracker.LoadFromStepMap(stepMap);
                _bindingFilesTracker.LoadFromStepMap(stepMap);
            }
        }

        public static bool IsProjectSupported(Project project)
        {
            return GetTargetLanguage(project) != ProgrammingLanguage.Other;
        }

        public static ProgrammingLanguage GetTargetLanguage(Project project)
        {
            if (project.FullName.EndsWith(".csproj"))
            {
                return ProgrammingLanguage.CSharp;
            }

            if (project.FullName.EndsWith(".vbproj"))
            {
                return ProgrammingLanguage.VB;
            }

            if (project.FullName.EndsWith(".fsproj"))
            {
                return ProgrammingLanguage.FSharp;
            }

            return ProgrammingLanguage.Other;
        }

        public static bool IsCodeFileSupported(ProjectItem projectItem)
        {
            var codeFileLanguage = GetCodeFileLanguage(projectItem);
            return codeFileLanguage != ProgrammingLanguage.Other && codeFileLanguage != ProgrammingLanguage.FSharp; //F# does not have code model
        }

        public static ProgrammingLanguage GetCodeFileLanguage(ProjectItem projectItem)
        {
            string name = projectItem.Name;
            if (name.EndsWith(".cs"))
            {
                return ProgrammingLanguage.CSharp;
            }

            if (name.EndsWith(".vb"))
            {
                return ProgrammingLanguage.VB;
            }

            if (name.EndsWith(".fs"))
            {
                return ProgrammingLanguage.FSharp;
            }

            return ProgrammingLanguage.Other;
        }
    }
}
