using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Sepidar.EntityFramework
{
    public class BufferManager
    {
        static object lockToken = new object();
        static Dictionary<string, List<object>> buffers = new Dictionary<string, List<object>>();
        static Dictionary<string, Action> flushMethods = new Dictionary<string, Action>();
        static Timer databaseBufferFlushTimer = null;
        static Dictionary<string, Action<object[]>> flushErrorHandlers = new Dictionary<string, Action<object[]>>();

        static BufferManager()
        {
            AppDomain.CurrentDomain.DomainUnload += FlushBuffers;
        }

        public static void AddToBuffer(string name, object @object, Action flusher)
        {
            lock (lockToken)
            {
                InitializeTimer();
                AddItem(name, @object);
                AddFlusher(name, flusher);
                EmptyBufferIfFull(name);
            }
        }

        public static void RegisterBufferFlushErrorHandler(string name, Action<object[]> handler)
        {
            lock (lockToken)
            {
                if (flushErrorHandlers.ContainsKey(name))
                {
                    flushErrorHandlers[name] = handler;
                }
                else
                {
                    flushErrorHandlers.Add(name, handler);
                }
            }
        }

        private static void AddItem(string name, object @object)
        {
            lock (lockToken)
            {
                if (buffers.ContainsKey(name))
                {
                    buffers[name] = buffers[name] ?? new List<object>();
                    buffers[name].Add(@object);
                }
                else
                {
                    buffers.Add(name, new List<object>());
                    buffers[name].Add(@object);
                }
            }
        }

        private static void EmptyBufferIfFull(string name)
        {
            if (buffers[name].Count >= Sepidar.Framework.Config.DatabaseBufferSize)
            {
                FlushBuffer(name);
            }
        }

        private static void FlushBuffer(string name)
        {
            lock (lockToken)
            {
                Action flusher;
                if (flushMethods.ContainsKey(name))
                {
                    flusher = flushMethods[name];
                }
                else
                {
                    Logger.LogError("No flusher method is assigned to buffer {0}. Buffer data is lost.".Fill(name));
                    return;
                }
                try
                {
                    var bufferCount = buffers[name].Count;
                    var watch = new Stopwatch();
                    Logger.LogInfo("Flushing {0} which has {1} records...".Fill(name, bufferCount));
                    watch.Start();
                    flusher.Invoke();
                    watch.Stop();
                    Logger.LogInfo("Flushed {0} which had {1} records. Took {2} milliseconds.".Fill(name, bufferCount, watch.ElapsedMilliseconds));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error while flushing buffer of {0}, which contained {1} items.".Fill(name, buffers[name].Count));
                    Logger.Log(ex);
                    CallFlushErrorHandler(name);
                }
            }
        }

        private static void CallFlushErrorHandler(string name)
        {
            lock (lockToken)
            {
                if (flushErrorHandlers.ContainsKey(name))
                {
                    Logger.LogInfo("Calling flush error handler, for {0}".Fill(name));
                    flushErrorHandlers[name].Invoke(buffers[name].ToArray());
                    Empty(name);
                }
                else
                {
                    Logger.LogWarning("Buffer {0} does not have a flush error handler registered for it. Data is lost. Please register a flush error handler.".Fill(name));
                }
            }
        }

        private static void AddFlusher(string name, Action flusher)
        {
            lock (lockToken)
            {
                if (flushMethods.ContainsKey(name))
                {
                    flushMethods[name] = flusher;
                }
                else
                {
                    flushMethods.Add(name, flusher);
                }
            }
        }

        private static void InitializeTimer()
        {
            if (databaseBufferFlushTimer == null)
            {
                databaseBufferFlushTimer = new Timer();
                databaseBufferFlushTimer.Interval = Config.DatabaseBufferFlushIntervalInMilliseconds;
                databaseBufferFlushTimer.Elapsed += TimerElapsed;
                databaseBufferFlushTimer.Enabled = true;
                databaseBufferFlushTimer.Start();
            }
        }

        private static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockToken)
            {
                databaseBufferFlushTimer.Stop();
                FlushBuffers(null, null);
                databaseBufferFlushTimer.Interval = Config.DatabaseBufferFlushIntervalInMilliseconds;
                databaseBufferFlushTimer.Start();
            }
        }

        public static List<object> Get(string name)
        {
            if (buffers.ContainsKey(name))
            {
                return buffers[name];
            }
            return new List<object>();
        }

        public static void Empty(string name)
        {
            lock (lockToken)
            {
                if (buffers.ContainsKey(name))
                {
                    buffers[name] = new List<object>();
                }
                else
                {
                    buffers.Add(name, new List<object>());
                }
            }
        }

        private static void FlushBuffers(object sender, EventArgs e)
        {
            lock (lockToken)
            {
                var names = buffers.Keys.ToList();
                foreach (var name in names)
                {
                    FlushBuffer(name);
                }
            }
        }
    }
}
