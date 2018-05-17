using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public abstract class RemoteGeneratorServices : GeneratorServices
    {
        private readonly IOutOfProcessTestGeneratorFactory _outOfProcessTestGeneratorFactory;
        private readonly IGeneratorInfoProvider _generatorInfoProvider;

        protected RemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, IOutOfProcessTestGeneratorFactory outOfProcessTestGeneratorFactory, IGeneratorInfoProvider generatorInfoProvider, IIdeTracer tracer, bool enableSettingsCache)
            : base(testGeneratorFactory, tracer, enableSettingsCache)
        {
            _outOfProcessTestGeneratorFactory = outOfProcessTestGeneratorFactory;
            _generatorInfoProvider = generatorInfoProvider;
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

            try
            {
                tracer.Trace(string.Format("Creating out of process remote wrapper for the project's generator ({0} at {1})", generatorInfo.GeneratorAssemblyVersion, generatorInfo.GeneratorFolder), "RemoteGeneratorServices");
                _outOfProcessTestGeneratorFactory.Setup(generatorInfo.GeneratorFolder);
                _outOfProcessTestGeneratorFactory.EnsureInitialized();
                return _outOfProcessTestGeneratorFactory;
            }
            catch (Exception exception)
            {
                tracer.Trace(exception.ToString(), "RemoteGeneratorServices");
                return null;
            }
        }
    }
}