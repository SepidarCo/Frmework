using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Sepidar.Framework
{
    public class CommandLine
    {
        ProcessStartInfo process;

        private CommandLine(string app)
        {
            process = new ProcessStartInfo(app);
        }

        public static CommandLine App(string app)
        {
            if (app != "cmd.exe" && !File.Exists(app))
            {
                throw new FrameworkException("{0} doesn't exist.".Fill(app));
            }
            return new CommandLine(app);
        }

        public CommandLine WithLog()
        {
            return this;
        }

        public static string Run(string command, bool log = false)
        {
            return CommandLine.App("cmd.exe").Execute(" /c " + command, log);
        }

        public string Execute(string command, bool log = false)
        {
            string result = "";
            process.Arguments = command;
            process.RedirectStandardInput = true;
            process.UseShellExecute = false;
            process.WindowStyle = ProcessWindowStyle.Hidden;
            process.CreateNoWindow = true;
            var shell = Process.Start(process);
            if (log)
            {
                process.RedirectStandardOutput = true;
                shell.OutputDataReceived += (sender, e) => result += e.Data + "\r\n";
            }
            shell.Start();
            if (log)
            {
                shell.BeginOutputReadLine();
            }
            shell.WaitForExit();
            //shell.Close();
            // We may not have received all the events yet!
            //Thread.Sleep(5000);
            return result;
        }
    }
}
