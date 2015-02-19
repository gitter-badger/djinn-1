using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sungiant.Cloud
{
    /// <summary>
    /// Utility class for running external processes.
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Run's command with arguments.
        /// </summary>
        static HashSet<Int32> runningProcessIds = new HashSet<Int32>();

        class AsyncOutputHandler
        {
            Action<String> commandLogger;

            public AsyncOutputHandler(Action<String> commandLogger)
            {
                this.commandLogger = commandLogger;
            }

            public void HandleOutput(object sendingProcess, DataReceivedEventArgs e)
            {
                if ( e.Data != null && this.commandLogger != null )
                {
                    var lines = e.Data.Split('\n');

                    foreach (String line in lines)
                    {
                        this.commandLogger(line);
                    }
                }
            }
        }

        /// <summary>
        /// Run's the given command with arguments and returns that
        /// command's return value when it is complete, interactive mode.
        /// </summary>
        public static Int32 Run(String command)
        {
            return Run(command, ProcessMode.Interactive, Console.WriteLine);
        }

        /// <summary>
        /// Run's the given command with arguments and returns that
        /// command's return value when it is complete, in normal mode.
        /// </summary>
        public static Int32 Run(String command, Action<String> commandLog)
        {
            return Run(command, ProcessMode.Normal, commandLog);
        }

        static Int32 Run(String command, ProcessMode mode, Action<String> commandLog)
        {
            if( commandLog != null )
                commandLog(command);

            var temp = command.Split (new Char[]{' '}, 2, StringSplitOptions.RemoveEmptyEntries);

            var process = new Process();

            process.StartInfo.FileName = temp[0];

            if (temp.Length > 1)
                process.StartInfo.Arguments = temp[1];

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.EnableRaisingEvents = true;

            // If we are not in interactive mode, asynchronously read the standard output
            if( mode == ProcessMode.Normal )
            {
                var asyncOutputHandler = new AsyncOutputHandler(commandLog);

                // Occurs when an application writes to its redirected StandardError stream.
                process.ErrorDataReceived += asyncOutputHandler.HandleOutput;

                // Occurs when an application writes to its redirected StandardOutput stream.
                process.OutputDataReceived += asyncOutputHandler.HandleOutput;
            }

            try
            {
                // Start the process.
                process.Start();

                // Keep track of the process id.
                lock (runningProcessIds)
                {
                    runningProcessIds.Add(process.Id);
                }

                if( mode == ProcessMode.Normal )
                {
                    // Begins asynchronous read operations on the redirected StandardOutput stream of the application.
                    process.BeginOutputReadLine();

                    // Begins asynchronous read operations on the redirected StandardError stream of the application.
                    process.BeginErrorReadLine();

                    // The application that is processing the asynchronous output should call the
                    // WaitForExit method to ensure that the output buffer has been flushed.
                    process.WaitForExit();
                }

                if( mode == ProcessMode.Interactive )
                {
                    char[] stdOutBuffer = new char[1];

                    while (!process.HasExited)
                    {
                        process.StandardOutput.Read(stdOutBuffer, 0, 1);
                        Console.Write(stdOutBuffer);
                    }

                    char[] stdErrBuffer = new char[2048];
                    process.StandardError.Read(stdErrBuffer, 0, 2048);
                    Console.Write(stdErrBuffer);
                }
            }
            catch (Exception ex)
            {
                if( commandLog != null )
                {
                    commandLog ("ProcessHelper.Run Exception: " + ex.Message);
                }
            }
            finally
            {
                lock (runningProcessIds)
                {
                    runningProcessIds.Remove(process.Id);
                }
            }

            return process.ExitCode;
        }

        /// <summary>
        /// Kills all child processes
        /// </summary>
        public static void KillAllProcesses()
        {
            lock (runningProcessIds)
            {
                foreach( Int32 processId in runningProcessIds )
                {
                    Process.GetProcessById(processId).Kill();
                }

                runningProcessIds.Clear();
            }
        }
    }
}

