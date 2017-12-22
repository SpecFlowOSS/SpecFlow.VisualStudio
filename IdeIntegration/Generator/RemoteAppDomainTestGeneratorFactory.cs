using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.RemoteAppDomain;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class RemoteAppDomainTestGeneratorFactory : IRemoteAppDomainTestGeneratorFactory
    {
        private const int APPDOMAIN_CLEANUP_SECONDS = 10;
        private const string LogCategory = "RemoteAppDomainTestGeneratorFactory";
        private readonly Info _info = new Info();

        private readonly IIdeTracer _tracer;

        private AppDomain _appDomain;
        private RemoteAppDomainResolver _remoteAppDomainResolver;
        private ITestGeneratorFactory _remoteTestGeneratorFactory;
        private ResolveOutput _resolveOutput;
        private UsageCounter _usageCounter;
        public Timer CleanupTimer;

        public RemoteAppDomainTestGeneratorFactory(IIdeTracer tracer)
        {
            _tracer = tracer;


            AppDomainCleanupTime = TimeSpan.FromSeconds(APPDOMAIN_CLEANUP_SECONDS);
            CleanupTimer = new Timer(CleanupTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        }

        public TimeSpan AppDomainCleanupTime { get; set; }

        public bool IsRunning
        {
            get { return _appDomain != null; }
        }

        public void Setup(string newGeneratorFolder)
        {
            if (_info.GeneratorFolder == newGeneratorFolder)
                return;

            Cleanup();
            _info.GeneratorFolder = newGeneratorFolder;
        }

        public void EnsureInitialized()
        {
            if (!IsRunning)
                Initialize();
        }

        public Version GetGeneratorVersion()
        {
            EnsureInitialized();
            try
            {
                _usageCounter.Increase();
                return _remoteTestGeneratorFactory.GetGeneratorVersion();
            }
            finally
            {
                _usageCounter.Decrease();
            }
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            EnsureInitialized();
            _usageCounter.Increase();
            var remoteGenerator = _remoteTestGeneratorFactory.CreateGenerator(projectSettings);

            var disposeNotificationGenerator = new DisposeNotificationTestGenerator(remoteGenerator);
            disposeNotificationGenerator.Disposed += () => _usageCounter.Decrease();
            return disposeNotificationGenerator;
        }

        public void Dispose()
        {
            Cleanup();
        }

        public void Cleanup()
        {
            if (!IsRunning)
                return;

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            _tracer.Trace("AssemblyResolve Event removed", LogCategory);
            _remoteAppDomainResolver.Dispose();

            _remoteTestGeneratorFactory = null;
            AppDomain.Unload(_appDomain);
            _appDomain = null;
            _tracer.Trace("AppDomain for generator disposed", LogCategory);
        }

        private void Initialize()
        {
            if (_info.GeneratorFolder == null)
                throw new InvalidOperationException(
                    "The RemoteAppDomainTestGeneratorFactory has to be configured with the Setup() method before initialization.");


            var appDomainSetup = new AppDomainSetup
            {
                ShadowCopyFiles = "true"
            };
            _appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);

            //var assemblyLocation = typeof(Info).Assembly.Location;
            //var rawAssembly = File.ReadAllBytes(assemblyLocation);
            //_appDomain.Load(rawAssembly);


            var testGeneratorFactoryTypeFullName = typeof(TestGeneratorFactory).FullName;
            Debug.Assert(testGeneratorFactoryTypeFullName != null);

            _tracer.Trace(string.Format("TestGeneratorFactory: {0}", testGeneratorFactoryTypeFullName), LogCategory);

            _tracer.Trace("AssemblyResolve Event added", LogCategory);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            var remoteAppDomainAssembly = typeof(RemoteAppDomainResolver).Assembly;
            _remoteAppDomainResolver = (RemoteAppDomainResolver) _appDomain.CreateInstanceFromAndUnwrap(remoteAppDomainAssembly.Location, typeof(RemoteAppDomainResolver).FullName,true, BindingFlags.Default, null, null, null, null);
            _remoteAppDomainResolver.Init(_info.GeneratorFolder);

            var generatorFactoryObject = _appDomain.CreateInstanceAndUnwrap(_info.RemoteGeneratorAssemblyName, testGeneratorFactoryTypeFullName);
            _remoteTestGeneratorFactory = generatorFactoryObject as ITestGeneratorFactory;

            if (_remoteTestGeneratorFactory == null)
                throw new InvalidOperationException("Could not load test generator factory.");

            _usageCounter = new UsageCounter(LoseReferences);
            _tracer.Trace("AppDomain for generator created", LogCategory);
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(new[] { ',' }, 2)[0];
            _tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: Name: {0}; ", args.Name), LogCategory);
            _tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Fullname: {0}", (args != null ? args.RequestingAssembly : null) != null ? args.RequestingAssembly.FullName : null), LogCategory);
            _tracer.Trace(string.Format("GeneratorAssemlbyResolveEvent: RequestingAssemlby.Location: {0}", args != null ? (args.RequestingAssembly != null ? args.RequestingAssembly.Location : null) : null), LogCategory);
            
            if (assemblyName.Equals(_info.RemoteGeneratorAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var testGeneratorFactoryAssembly = typeof(ITestGeneratorFactory).Assembly;

                _tracer.Trace(string.Format("TestGeneratorFactoryAssembly resolved to {0}", testGeneratorFactoryAssembly != null ? testGeneratorFactoryAssembly.Location : null), LogCategory);
                return testGeneratorFactoryAssembly;
            }

            if (assemblyName.Equals(_info.RemoteRuntimeAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                var specFlowAssembly = typeof(SpecFlowException).Assembly;
                _tracer.Trace(string.Format("SpecFlowAssembly resolved to {0}", specFlowAssembly != null ? specFlowAssembly.Location : null), LogCategory);

                return specFlowAssembly;
            }

            var packagePath = Path.Combine(_info.GeneratorFolder, assemblyName + ".dll");
            if (File.Exists(packagePath))
            {
                return Assembly.LoadFile(packagePath);
            }

            var extensionPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), assemblyName + ".dll");
            if (File.Exists(extensionPath))
            {
                return Assembly.LoadFile(extensionPath);
            }

            return null;
        }

        private void LoseReferences()
        {
            CleanupTimer.Change(AppDomainCleanupTime, TimeSpan.FromMilliseconds(-1));
        }

        private void CleanupTimerElapsed(object state)
        {
            if (!_usageCounter.HasRefernece)
                Cleanup();
        }

        private class UsageCounter
        {
            private readonly Action cleanup;
            private int counter;

            public UsageCounter(Action cleanup)
            {
                this.cleanup = cleanup;
            }

            public bool HasRefernece
            {
                get { return counter > 0; }
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

        private class DisposeNotificationTestGenerator : ITestGenerator
        {
            private readonly ITestGenerator innerGenerator;

            public DisposeNotificationTestGenerator(ITestGenerator innerGenerator)
            {
                this.innerGenerator = innerGenerator;
            }

            public void Dispose()
            {
                innerGenerator.Dispose();
                if (Disposed != null)
                    Disposed();
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

            public event Action Disposed;

            public override string ToString()
            {
                return innerGenerator.ToString();
            }
        }
    }

    [Serializable]
    internal class ResolveOutput : MarshalByRefObject
    {
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            Debug.WriteLine(args.Name + ", " + args.RequestingAssembly.FullName);
            return null;
        }
    }
}