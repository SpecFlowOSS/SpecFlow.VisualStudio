using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.RemoteAppDomain;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class OutOfProcessTestGeneratorFactory : IOutOfProcessTestGeneratorFactory
    {
        private readonly IIdeTracer _tracer;
        private readonly Info _info = new Info();

        public OutOfProcessTestGeneratorFactory(IIdeTracer tracer)
        {
            _tracer = tracer;
        }

        public Version GetGeneratorVersion()
        {
            return new Version();
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            return new OutOfProcessTestGenerator(_info, projectSettings);
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
