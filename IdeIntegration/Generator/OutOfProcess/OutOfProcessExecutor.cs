using System.IO;
using CommandLine;
using TechTalk.SpecFlow.RemoteAppDomain;
using TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.Parameters;

namespace TechTalk.SpecFlow.IdeIntegration.Generator.OutOfProcess
{
    class OutOfProcessExecutor
    {
        private readonly Info _info;
        private readonly string _fullPathToExe;
        private const string ExeName = "TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe";


        public OutOfProcessExecutor(Info info)
        {
            _info = info;
            string currentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            _fullPathToExe = Path.Combine(currentDirectory, ExeName);
        }

        public Result Execute(CommonParameters commonParameters)
        {
            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(commonParameters);

            var processHelper = new ProcessHelper();

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, _fullPathToExe, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            return new Result(exitCode, outputFileContent);
        }
    }
}
