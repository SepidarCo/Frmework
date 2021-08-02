using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;

namespace Sepidar.Framework
{
    public static class Logger
    {
        static object lockToken = new object();

        static int loggingBufferCounter = 0;

        static Stopwatch loggingBufferOverflowTimer = new Stopwatch();

        static List<string> loggingBufferList = new List<string>();

        public static void Log(dynamic @object, MessageType type)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), @object);
            Console.ForegroundColor = ConsoleColor.White;
            if (@object is string)
            {
                @object.Insert(0, "{0}: ".Fill(type));
            }
            try
            {
                if (LogEntryShouldBeLoggedBasedOnVerbositySettings(type))
                {
                    if (ShouldLogViaBufferAndFlush())
                    {
                        LogToFileAsBulk(type, @object);
                    }
                    else
                    {
                        LogToFile(type, @object);
                    }
                    LogToDatabase(type, @object);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //throw new FrameworkException(ex.ToString());
            }
        }

        //private static bool LogEntryShouldBeLoggedBasedOnVerbositySettings(MessageType type)
        //{
        //    return (int)Config.LogVerbosity >= (int)type.ToString().ToEnum<MessageType>();
        //}

        /// <summary>
        /// این قسمت رو باید درست کنی بعدا فعلا همینطوری این رو گذاشتی که مقدار تایپ رو برگردونه 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool LogEntryShouldBeLoggedBasedOnVerbositySettings(MessageType type)
        {
            return (int)Config.LogVerbosity >= (int)type;
        }

        public static bool ShouldLogViaBufferAndFlush()
        {
            return Config.HasSetting("LogBufferSize") && Config.HasSetting("LogBufferFlushIntervalInMilliseconds");
        }

        public static bool ShouldLogsBeSepratredInDifferentFilesByType()
        {
            if (Config.HasSetting("Log:ShouldBeSepratredInDifferentFilesByType"))
            {
                return Config.GetSetting("Log:ShouldBeSepratredInDifferentFilesByType").ToBoolean();
            }
            return Config.HasSetting("Log:ShouldBeSepratredInDifferentFilesByType");
        }
        internal static void LogToFile(MessageType type, dynamic @object)
        {
            var text = CreateLogEntry(type, @object);
            string logPath;
            lock (lockToken)
            {
                if (ShouldLogsBeSepratredInDifferentFilesByType())
                    logPath = FindLogPath(type);
                else
                    logPath = FindLogPath();
                File.AppendAllText(logPath, text);
            }
        }

        private static dynamic CreateLogEntry(MessageType type, dynamic @object)
        {
            return string.Format("\r\n{0}-{1}: {2}", DateTime.Now, type.ToString(), @object);
        }

        private static void LogToDatabase(MessageType type, dynamic @object)
        {
        }

        private static void LogToFileAsBulk(MessageType type, dynamic @object)
        {
            lock (lockToken)
            {
                LogBuffer.AddToBuffer(CreateLogEntry(type, @object), type);
            }
        }

        public static string FindLogPath()
        {
            string logPath = string.Format(Path.Combine(Config.LogFolderPath, DateTime.Now.ToPersianDate().Replace("/", "-") + ".txt"));
            CreateDirectoryIfNotExist(logPath);
            return logPath;
        }

        public static string FindLogPath(MessageType messageType)
        {
            string logPath = string.Format(Path.Combine(Config.LogFolderPath, "{0}-{1}.txt".Fill(DateTime.Now.ToPersianDate().Replace("/", "-"), messageType.ToString())));
            CreateDirectoryIfNotExist(logPath);
            return logPath;
        }

        private static void CreateDirectoryIfNotExist(string logPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(logPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            }
        }

        public static void Log(this Exception ex)
        {
            LogError(ExceptionHelper.BuildExceptionString(ex));
        }

        public static void LogError(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(text, MessageType.Error);
        }

        public static void LogInfo(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Log(text, MessageType.Info);
        }

        public static void LogInfo(dynamic obj)
        {
            Log(obj, MessageType.Info);
        }

        public static void LogSuccess(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log(text, MessageType.Success);
        }

        public static void LogWarning(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(text, MessageType.Warning);
        }

        public static void LogActivity(this string text)
        {
            LogActivity(text, new byte[] { });
        }

        public static void LogActivity(this string text, byte[] data)
        {

        }

        public static void LogPerformance(dynamic @object)
        {
            Logger.LogInfo(JsonConvert.SerializeObject(@object));
        }

        public static void Count(int number)
        {
            Console.Write("\r                 ");
            Console.Write("\r" + number);
        }
    }
}
