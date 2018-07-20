using System;
using System.Diagnostics;
using CommandLine;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    public class OutOfProcessTestGeneratorFactory : IOutOfProcessTestGeneratorFactory
    {
        private readonly IIdeTracer _tracer;
        private readonly IntegrationOptions _integrationOptions;
        private readonly Info _info = new Info();
        private OutOfProcessExecutor _outOfProcessExecutor;

        public OutOfProcessTestGeneratorFactory(IIdeTracer tracer, IntegrationOptions integrationOptions)
        {
            _tracer = tracer;
            _integrationOptions = integrationOptions;
            _outOfProcessExecutor = new OutOfProcessExecutor(_info, integrationOptions);
        }

        public Version GetGeneratorVersion()
        {
            var result = _outOfProcessExecutor.Execute(new GetGeneratorVersionParameters()
            {
                Debug = Debugger.IsAttached
            });

            return Version.Parse(result.Output);
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            return new OutOfProcessTestGenerator(_info, projectSettings, _integrationOptions);
        }

        public void Dispose()
        {
            Cleanup();
        }

        public bool IsRunning { get; private set; }
        public void Setup(string newGeneratorFolder)
        {
            if (_info.GeneratorFolder == newGeneratorFolder)
                return;

            Cleanup();
            _info.GeneratorFolder = newGeneratorFolder;
        }

        public void EnsureInitialized()
        {
            
        }

        public void Cleanup()
        {
            
        }
    }
}
