using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    class Result
    {
        public Result(int exitCode, string output)
        {
            ExitCode = exitCode;
            Output = output;
        }

        public int ExitCode { get; private set; }
        public string Output { get; private set; }
    }

    class OutOfProcessExecutor
    {
        private readonly Info _info;
        private const string ExeName = "TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe";


        public OutOfProcessExecutor(Info info)
        {
            _info = info;
        }

        public Result Execute(CommonParameters commonParameters)
        {
            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(commonParameters);

            var processHelper = new ProcessHelper();

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, ExeName, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            return new Result(exitCode, outputFileContent);
        }
    }
}
