using System;
using System.IO;
using CommandLine;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    class OutOfProcessExecutor
    {
        private readonly Info _info;
        private readonly string _fullPathToExe;
        private const string ExeName = "TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe";


        public OutOfProcessExecutor(Info info, IntegrationOptions integrationOptions)
        {
            _info = info;
            string currentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            if (String.IsNullOrWhiteSpace(integrationOptions.CodeBehindFileGeneratorPath))
            {
                _fullPathToExe = Path.Combine(currentDirectory, ExeName);
            }
            else
            {
                _fullPathToExe = integrationOptions.CodeBehindFileGeneratorPath;
            }
        }

        public Result Execute(CommonParameters commonParameters)
        {
            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(commonParameters);

            var processHelper = new ProcessHelper();

            if (!File.Exists(_fullPathToExe))
            {
                return new Result(1, string.Format("Could not find CodeBehindGenerator binary at location {0}." + Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/", _fullPathToExe));
            }

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, _fullPathToExe, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            return new Result(exitCode, outputFileContent);
        }
    }
}
