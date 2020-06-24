using System;
using System.Collections.Generic;
using System.Diagnostics;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    public class OutOfProcessTestGeneratorFactory : IOutOfProcessTestGeneratorFactory
    {
        private readonly Info _info = new Info();
        private readonly OutOfProcessExecutor _outOfProcessExecutor;
        private readonly IntegrationOptions _integrationOptions;

        public OutOfProcessTestGeneratorFactory(IntegrationOptions integrationOptions)
        {
            _integrationOptions = integrationOptions;
            _outOfProcessExecutor = new OutOfProcessExecutor(_info, integrationOptions);
        }

        public Version GetGeneratorVersion()
        {
            var result = _outOfProcessExecutor.Execute(new GetGeneratorVersionParameters()
            {
                Debug = Debugger.IsAttached
            },false);

            return Version.Parse(result.Output);
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings, IEnumerable<GeneratorPluginInfo> generatorPlugins)
        {
            return new OutOfProcessTestGenerator(_info, projectSettings, _integrationOptions);
        }

        public bool IsRunning { get; private set; }
        public void Setup(string newGeneratorFolder)
        {
            if (_info.GeneratorFolder == newGeneratorFolder)
                return;

            _info.GeneratorFolder = newGeneratorFolder;
        }

        public void EnsureInitialized()
        {
            
        }
    }
}
