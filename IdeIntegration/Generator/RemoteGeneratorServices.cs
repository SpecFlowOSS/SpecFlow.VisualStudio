using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator.AppDomain;
using TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public abstract class RemoteGeneratorServices : GeneratorServices
    {
        private readonly IRemoteAppDomainTestGeneratorFactory _remoteAppDomainTestGeneratorFactory;
        private readonly IOutOfProcessTestGeneratorFactory _outOfProcessTestGeneratorFactory;
        private readonly IGeneratorInfoProvider _generatorInfoProvider;

        protected RemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, IRemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory, IOutOfProcessTestGeneratorFactory outOfProcessTestGeneratorFactory, IGeneratorInfoProvider generatorInfoProvider, IIdeTracer tracer, bool enableSettingsCache)
            : base(testGeneratorFactory, tracer, enableSettingsCache)
        {
            _remoteAppDomainTestGeneratorFactory = remoteAppDomainTestGeneratorFactory;
            _outOfProcessTestGeneratorFactory = outOfProcessTestGeneratorFactory;
            _generatorInfoProvider = generatorInfoProvider;

            UseOutOfProcess = true;
        }

        protected virtual GeneratorInfo GetGeneratorInfo()
        {
            return _generatorInfoProvider.GetGeneratorInfo();
        }

        protected Version GetCurrentGeneratorAssemblyVersion()
        {
            return typeof(TestGeneratorFactory).Assembly.GetName().Version;
        }

        protected override ITestGeneratorFactory GetTestGeneratorFactoryForCreate()
        {
            var generatorInfo = GetGeneratorInfo();
            if (generatorInfo == null || generatorInfo.GeneratorAssemblyVersion == null || generatorInfo.GeneratorFolder == null)
            {
                // we don't know about the generator -> call the "current" directly
                tracer.Trace("Unable to detect generator location: the generator bound to the IDE is used", "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            var currentGeneratorAssemblyVersion = GetCurrentGeneratorAssemblyVersion();

            if (generatorInfo.GeneratorAssemblyVersion < new Version(1, 6) && 
                generatorInfo.GeneratorAssemblyVersion != currentGeneratorAssemblyVersion) // in debug mode 1.0 is the version, that is < 1.6
            {
                // old generator version -> call the "current" directly
                tracer.Trace(string.Format("The project's generator ({0}) is older than v1.6: the generator bound to the IDE is used", generatorInfo.GeneratorAssemblyVersion), "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            if (generatorInfo.GeneratorAssemblyVersion == currentGeneratorAssemblyVersion && !generatorInfo.UsesPlugins)
            {
                // uses the "current" generator (and no plugins) -> call it directly
                tracer.Trace("The generator of the project is the same as the generator bound to the IDE: using it from the IDE", "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            try
            {
                if (UseOutOfProcess)
                {
                    tracer.Trace(string.Format("Creating out of process remote wrapper for the project's generator ({0} at {1})", generatorInfo.GeneratorAssemblyVersion, generatorInfo.GeneratorFolder), "RemoteGeneratorServices");
                    _outOfProcessTestGeneratorFactory.Setup(generatorInfo.GeneratorFolder);
                    _outOfProcessTestGeneratorFactory.EnsureInitialized();
                    return _outOfProcessTestGeneratorFactory;
                }
                else
                {
                    tracer.Trace(string.Format("Creating appdomain remote wrapper for the project's generator ({0} at {1})", generatorInfo.GeneratorAssemblyVersion, generatorInfo.GeneratorFolder), "RemoteGeneratorServices");
                    _remoteAppDomainTestGeneratorFactory.Setup(generatorInfo.GeneratorFolder);
                    _remoteAppDomainTestGeneratorFactory.EnsureInitialized();
                    return _remoteAppDomainTestGeneratorFactory;
                }

                
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "RemoteGeneratorServices");
                // there was an error -> call the "current" directly (plus cleanup)
                Cleanup();
                return base.GetTestGeneratorFactoryForCreate();
            }
        }

        public bool UseOutOfProcess { get; set; } 

        public override void InvalidateSettings()
        {
            Cleanup();

            base.InvalidateSettings();
        }

        public override void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _remoteAppDomainTestGeneratorFactory.Cleanup();
        }
    }
}