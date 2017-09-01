using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class RemoteAppDomainTestGeneratorFactory : IRemoteAppDomainTestGeneratorFactory
    {
        private const int APPDOMAIN_CLEANUP_SECONDS = 10;
        private const string LogCategory = "RemoteAppDomainTestGeneratorFactory";

        private readonly IIdeTracer tracer;

        private string generatorFolder;
        private AppDomain appDomain = null;
        private ITestGeneratorFactory remoteTestGeneratorFactory = null;
        private UsageCounter usageCounter;
        private readonly string remoteGeneratorAssemblyName;
        private readonly string remoteRuntimeAssemblyName;
        public Timer cleanupTimer;

        private class UsageCounter
        {
            private int counter = 0;
            private readonly Action cleanup;

            public bool HasRefernece { get { return counter > 0; } }

            public UsageCounter(Action cleanup)
            {
                this.cleanup = cleanup;
            }

            public void Increase()
            {
                counter++;
            }

            public void Decrease()
            {
                if (--counter <= 0)
                    cleanup();
            }
        }

        public bool IsRunning
        {
            get { return appDomain != null; }
        }

        public TimeSpan AppDomainCleanupTime { get; set; }

        public RemoteAppDomainTestGeneratorFactory(IIdeTracer tracer)
        {
            this.tracer = tracer;
            this.remoteGeneratorAssemblyName = typeof(ITestGeneratorFactory).Assembly.GetName().Name;
            this.remoteRuntimeAssemblyName = typeof(SpecFlowException).Assembly.GetName().Name;

            this.AppDomainCleanupTime = TimeSpan.FromSeconds(APPDOMAIN_CLEANUP_SECONDS);
            this.cleanupTimer = new Timer(CleanupTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Setup(string newGeneratorFolder)
        {
            if (generatorFolder == newGeneratorFolder)
                return;

            Cleanup();
            generatorFolder = newGeneratorFolder;
        }

        public void EnsureInitialized()
        {
            if (!IsRunning)
                Initialize();
        }

        private void Initialize()
        {
            if (generatorFolder == null)
                throw new InvalidOperationException("The RemoteAppDomainTestGeneratorFactory has to be configured with the Setup() method before initialization.");

            
            AppDomainSetup appDomainSetup = new AppDomainSetup { ApplicationBase = generatorFolder };
            appDomainSetup.ShadowCopyFiles = "true";

            //set configuration for generator app domain to have assembly redirects
            var appConfigFile = Path.Combine(generatorFolder, "plugincompability.config");
            if (File.Exists(appConfigFile))
            {
                appDomainSetup.ConfigurationFile = appConfigFile;
            }
            tracer.Trace(string.Format("AppDomainSetup - ApplicationBase: {0}; ConfigFile: {1}", generatorFolder, appDomainSetup.ConfigurationFile ?? "not specified"), LogCategory);

            appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);

            var testGeneratorFactoryTypeFullName = typeof(TestGeneratorFactory).FullName;
            Debug.Assert(testGeneratorFactoryTypeFullName != null);

            tracer.Trace(string.Format("TestGeneratorFactory: {0}", testGeneratorFactoryTypeFullName), LogCategory);

            tracer.Trace("AssemblyResolve Event added", LogCategory);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            var generatorFactoryObject = appDomain.CreateInstanceAndUnwrap(remoteGeneratorAssemblyName, testGeneratorFactoryTypeFullName);
            remoteTestGeneratorFactory = generatorFactoryObject as ITestGeneratorFactory;

            if (remoteTestGeneratorFactory == null)
                throw new InvalidOperationException("Could not load test generator factory.");

            usageCounter = new UsageCounter(LoseReferences);
            tracer.Trace("AppDomain for generator created", LogCategory);
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: Name: {0}; ", args.Name), LogCategory);
            tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Fullname: {0}", (args != null ? args.RequestingAssembly : null) != null ? args.RequestingAssembly.FullName : null), LogCategory);
            tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Location: {0}", args != null ? (args.RequestingAssembly != null ? args.RequestingAssembly.Location : null) : null), LogCategory);

            string assemblyName = args.Name.Split(new[] {','}, 2)[0];
            if (assemblyName.Equals(remoteGeneratorAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var testGeneratorFactoryAssembly = typeof(ITestGeneratorFactory).Assembly;

                tracer.Trace(string.Format("TestGeneratorFactoryAssembly resolved to {0}", testGeneratorFactoryAssembly != null ? testGeneratorFactoryAssembly.Location : null), LogCategory);
                return testGeneratorFactoryAssembly;
            }
            if (assemblyName.Equals(remoteRuntimeAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var specFlowAssembly = typeof(SpecFlowException).Assembly;
                tracer.Trace(string.Format("SpecFlowAssembly resolved to {0}", specFlowAssembly != null ? specFlowAssembly.Location : null), LogCategory);

                return specFlowAssembly;
            }
            return null;
        }

        public Version GetGeneratorVersion()
        {
            EnsureInitialized();
            try
            {
                usageCounter.Increase();
                return remoteTestGeneratorFactory.GetGeneratorVersion();
            }
            finally
            {
                usageCounter.Decrease();
            }
        }

        private class DisposeNotificationTestGenerator : ITestGenerator
        {
            private readonly ITestGenerator innerGenerator;
            public event Action Disposed;

            public DisposeNotificationTestGenerator(ITestGenerator innerGenerator)
            {
                this.innerGenerator = innerGenerator;
            }

            public void Dispose()
            {
                innerGenerator.Dispose();
                if (Disposed != null)
                {
                    Disposed();
                }
            }

            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                return innerGenerator.GenerateTestFile(featureFileInput, settings);
            }

            public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
            {
                return innerGenerator.DetectGeneratedTestVersion(featureFileInput);
            }

            public string GetTestFullPath(FeatureFileInput featureFileInput)
            {
                return innerGenerator.GetTestFullPath(featureFileInput);
            }

            public override string ToString()
            {
                return innerGenerator.ToString();
            }
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            EnsureInitialized();
            usageCounter.Increase();
            var remoteGenerator = remoteTestGeneratorFactory.CreateGenerator(projectSettings);

            var disposeNotificationGenerator = new DisposeNotificationTestGenerator(remoteGenerator);
            disposeNotificationGenerator.Disposed += () => usageCounter.Decrease();
            return disposeNotificationGenerator;
        }

        public void Dispose()
        {
            Cleanup();
        }

        private void LoseReferences()
        {
            cleanupTimer.Change(AppDomainCleanupTime, TimeSpan.FromMilliseconds(-1));
        }

        private void CleanupTimerElapsed(object state)
        {
            if (!usageCounter.HasRefernece)
                Cleanup();
        }

        public void Cleanup()
        {
            if (!IsRunning)
                return;

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            tracer.Trace("AssemblyResolve Event removed", LogCategory);

            remoteTestGeneratorFactory = null;
            AppDomain.Unload(appDomain);
            appDomain = null;
            tracer.Trace("AppDomain for generator disposed", LogCategory);
        }
    }
}
