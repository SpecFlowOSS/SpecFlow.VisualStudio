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
        private readonly IntegrationOptions _integrationOptions;
        private readonly string _fullPathToExe;
        private const string ExeName = "TechTalk.SpecFlow.VisualStudio.CodeBehindGenerator.exe";


        public OutOfProcessExecutor(Info info, IntegrationOptions integrationOptions)
        {
            _info = info;
            _integrationOptions = integrationOptions;
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

        public Result Execute(CommonParameters commonParameters, bool transferViaFile)
        {
            commonParameters.OutputDirectory = String.IsNullOrWhiteSpace(_integrationOptions.CodeBehindFileGeneratorExchangePath) ? Path.GetTempPath() : _integrationOptions.CodeBehindFileGeneratorExchangePath;


            string commandLineParameters = CommandLine.Parser.Default.FormatCommandLine(commonParameters);

            var processHelper = new ProcessHelper();

            if (!File.Exists(_fullPathToExe))
            {
                return new Result(1, string.Format("Could not find CodeBehindGenerator binary at location {0}." + Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/", _fullPathToExe));
            }

            int exitCode = processHelper.RunProcess(_info.GeneratorFolder, _fullPathToExe, commandLineParameters);

            var outputFileContent = processHelper.ConsoleOutput;

            outputFileContent = FilterConfigDebugOutput(outputFileContent);


            if (transferViaFile)
            {
                var firstLine = outputFileContent.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (firstLine != null)
                {
                    if (File.Exists(firstLine))
                    {
                        outputFileContent = File.ReadAllText(firstLine, Encoding.UTF8);
                        try
                        {
                            File.Delete(firstLine);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    else
                    {
                        return new Result(1, "We could not find a data exchange file at the path " + firstLine + "" + Environment.NewLine +
                                             Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/" +
                                             "Complete output: " + Environment.NewLine +
                                             outputFileContent + Environment.NewLine+
                                             Environment.NewLine+
                                             "Command: " + _fullPathToExe + Environment.NewLine + 
                                             "Parameters: " + commonParameters + Environment.NewLine +
                                             "Working Directory: " + _info.GeneratorFolder);
                    }
                }
                else
                {
                    return new Result(1, "Data Exchange via file did not worked, because we didn't receive a file path to read. " + Environment.NewLine +
                                         Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/" + 
                                         "Complete output: " + Environment.NewLine +
                                         outputFileContent + Environment.NewLine +
                                         Environment.NewLine +
                                         "Command: " + _fullPathToExe + Environment.NewLine +
                                         "Parameters: " + commonParameters + Environment.NewLine +
                                         "Working Directory: " + _info.GeneratorFolder);
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
