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
            var exchangePath = String.IsNullOrWhiteSpace(_integrationOptions.CodeBehindFileGeneratorExchangePath)
                ? Path.GetTempPath()
                : _integrationOptions.CodeBehindFileGeneratorExchangePath;
            commonParameters.OutputDirectory = Path.GetDirectoryName(PathAddBackslash(exchangePath));


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
                                             Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/" + Environment.NewLine +
                                             "Complete output: " + Environment.NewLine +
                                             outputFileContent + Environment.NewLine+
                                             Environment.NewLine+
                                             "Command: " + _fullPathToExe + Environment.NewLine + 
                                             "Parameters: " + commandLineParameters + Environment.NewLine +
                                             "Working Directory: " + _info.GeneratorFolder);
                    }
                }
                else
                {
                    return new Result(1, "Data Exchange via file did not worked, because we didn't receive a file path to read. " + Environment.NewLine +
                                         Environment.NewLine + "Please open an issue at https://github.com/techtalk/SpecFlow/issues/" + Environment.NewLine +
                                         "Complete output: " + Environment.NewLine +
                                         outputFileContent + Environment.NewLine +
                                         Environment.NewLine +
                                         "Command: " + _fullPathToExe + Environment.NewLine +
                                         "Parameters: " + commandLineParameters + Environment.NewLine +
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


        string PathAddBackslash(string path)
        {
            // They're always one character but EndsWith is shorter than
            // array style access to last path character. Change this
            // if performance are a (measured) issue.
            string separator1 = Path.DirectorySeparatorChar.ToString();
            string separator2 = Path.AltDirectorySeparatorChar.ToString();

            // Trailing white spaces are always ignored but folders may have
            // leading spaces. It's unusual but it may happen. If it's an issue
            // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
            path = path.TrimEnd();

            // Argument is always a directory name then if there is one
            // of allowed separators then I have nothing to do.
            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            // If there is the "alt" separator then I add a trailing one.
            // Note that URI format (file://drive:\path\filename.ext) is
            // not supported in most .NET I/O functions then we don't support it
            // here too. If you have to then simply revert this check:
            // if (path.Contains(separator1))
            //     return path + separator1;
            //
            // return path + separator2;
            if (path.Contains(separator2))
                return path + separator2;

            // If there is not an "alt" separator I add a "normal" one.
            // It means path may be with normal one or it has not any separator
            // (for example if it's just a directory name). In this case I
            // default to normal as users expect.
            return path + separator1;
        }

    }
}
