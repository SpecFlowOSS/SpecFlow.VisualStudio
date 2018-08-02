using System;
using System.IO;
using System.Linq;
using System.Text;
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

            if (!File.Exists(_fullPathToExe))
            {
                return new Result(1, string.Format("Could not find CodeBehindGenerator binary at location {0}." + Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/", _fullPathToExe));
            }

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, _fullPathToExe, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            outputFileContent = FilterConfigDebugOutput(outputFileContent);

            var firstLine = outputFileContent
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (firstLine != null)
            {
                if (File.Exists(firstLine))
                {
                    outputFileContent = File.ReadAllText(firstLine, Encoding.UTF8);
                }
            }


            return new Result(exitCode, outputFileContent);
        }

        private string FilterConfigDebugOutput(string result)
        {
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var output = new StringBuilder();

            foreach (string line in lines)
            {
                if (line.Contains("Using default config") || line.Contains("Using app.config") || line.Contains("Using specflow.json"))
                {
                    continue;
                }

                output.AppendLine(line);
            }

            return output.ToString();
        }

    }
}
