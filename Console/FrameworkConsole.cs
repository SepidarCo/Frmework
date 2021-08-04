using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Diagnostics;
using System.IO;

namespace Sepidar.Console
{
    public static class FrameworkConsole
    {
        public class PreMainAttribute : Attribute
        {
            static PreMainAttribute()
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            }

            private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                if (e.ExceptionObject != null)
                {
                    ((Exception)e.ExceptionObject).Log();
                }
            }
        }

        public static void SetSize()
        {
            try
            {
                System.Console.WindowWidth = ConsoleConfig.Width;
                System.Console.WindowHeight = ConsoleConfig.Height;
                System.Console.BufferHeight = ConsoleConfig.Records;
                System.Console.BufferWidth = ConsoleConfig.Wrap ? ConsoleConfig.Width : 300;
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("The handle is invalid."))
                {
                    throw new FrameworkException("This is not a console application");
                }
                throw;
            }
        }

        public static void Run(string path, MethodInvocation methodInvocation)
        {
            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, methodInvocation.JsonSerialize());
            //Process process = new Process();
            //process.StartInfo.FileName = path;
            //process.StartInfo.Arguments = fileName;
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //process.Start();
            //process.WaitForExit();
            var console = Process.Start(path, fileName);
        }
    }
}
