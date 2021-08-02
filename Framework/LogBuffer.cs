using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Sepidar.Framework
{
    public static class LogBuffer
    {
        static object lockToken = new object();
        static Timer timer = null;
        static List<Tuple<string, MessageType>> buffer = new List<Tuple<string, MessageType>>();

        static LogBuffer()
        {
            AppDomain.CurrentDomain.DomainUnload += FlushBuffer;
        }

        public static void AddToBuffer(dynamic @object, MessageType messageType)
        {
            lock (lockToken)
            {
                InitializeTimer();
                buffer.Add(new Tuple<string, MessageType>(@object, messageType));
                EmptyBufferIfFull();
            }
        }

        private static void EmptyBufferIfFull()
        {
            lock (lockToken)
            {
                if (buffer.Count >= Convert.ToInt32(Config.LogBufferSize))
                    FlushBuffer(null, null);
            }
        }

        private static void InitializeTimer()
        {
            if (timer == null)
            {
                timer = new Timer
                {
                    Interval = Config.LogBufferFlushIntervalInMilliseconds,
                    Enabled = true
                };
                timer.Elapsed += TimerElapsed;
                timer.Start();
            }
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockToken)
            {
                timer.Stop();
                FlushBuffer(null, null);
                timer.Interval = Config.LogBufferFlushIntervalInMilliseconds;
                timer.Start();
            }
        }

        private static void FlushBuffer(object sender, EventArgs e)
        {
            lock (lockToken)
            {
                if (buffer.Count == 0)
                    return;
                if (!Logger.ShouldLogsBeSepratredInDifferentFilesByType())
                {
                    string logPath = Logger.FindLogPath();
                    File.AppendAllText(logPath, string.Join("\r\n", buffer.Select(i => i.Item1).ToArray()));
                }
                else
                {
                    foreach (MessageType type in Enum.GetValues(typeof(MessageType)))
                        File.AppendAllText(Logger.FindLogPath(type), string.Join("\r\n", buffer.Where(i => i.Item2 == type).Select(i => i.Item1).ToArray()));
                }
                buffer.Clear();
            }
        }
    }
}
