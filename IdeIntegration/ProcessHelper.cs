using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace TechTalk.SpecFlow.IdeIntegration
{
    public class ProcessHelper
    {
        private static TimeSpan _timeout = TimeSpan.FromMinutes(10);
        private static int _timeOutInMilliseconds = Convert.ToInt32(_timeout.TotalMilliseconds);

        public string ConsoleOutput { get; private set; }

        public int RunProcess(string workingDirectory, string executablePath, string argumentsFormat, params object[] arguments)
        {
            var parameters = string.Format(argumentsFormat, arguments);

            Console.WriteLine("Starting external program: \"{0}\" {1}", executablePath, parameters);
            ProcessStartInfo psi = new ProcessStartInfo(executablePath, parameters);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.WorkingDirectory = workingDirectory;


            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true,

            };

            StringBuilder output = new StringBuilder();


            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            {
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };


                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(_timeOutInMilliseconds) &&
                        outputWaitHandle.WaitOne(_timeOutInMilliseconds) &&
                        errorWaitHandle.WaitOne(_timeOutInMilliseconds))
                    {
                        ConsoleOutput = output.ToString();
                    }
                    else
                    {
                        throw new TimeoutException(string.Format("Process {0} {1} took longer than {2} min to complete", psi.FileName, psi.Arguments, _timeout.TotalMinutes));
                    }

                }
            }
            Console.WriteLine(ConsoleOutput);


            return process.ExitCode;
        }
    }
}
