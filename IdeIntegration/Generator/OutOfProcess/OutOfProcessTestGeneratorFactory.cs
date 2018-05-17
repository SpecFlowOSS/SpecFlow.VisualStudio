using System;
using System.Diagnostics;
using CommandLine;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    public class OutOfProcessTestGeneratorFactory : IOutOfProcessTestGeneratorFactory
    {
        private readonly Info _info = new Info();
        private readonly OutOfProcessExecutor _outOfProcessExecutor;

        public OutOfProcessTestGeneratorFactory()
        {
            _outOfProcessExecutor = new OutOfProcessExecutor(_info);
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
            return new OutOfProcessTestGenerator(_info, projectSettings);
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
