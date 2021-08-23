using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;

namespace Sepidar.WindowsService
{
    public class HostedService : IHostedService, IDisposable
    {
        private Thread jobThread;
        private static bool isUpAndRunning = true;

        public string ServiceName { get; set; }

        public static bool IsUpAndRunning
        {
            get { return isUpAndRunning; }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInfo("Starting {0}...".Fill(ServiceName));
                jobThread = new Thread(() =>
                {
                    try
                    {
                        // check the environment
                        StartDoingTheJob();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error in starting service {0}".Fill(ServiceName));
                        Logger.Log(ex);
                    }
                });
                jobThread.Start();
                Logger.LogInfo("Started {0}".Fill(ServiceName));
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in starting service {0}".Fill(ServiceName));
                Logger.Log(ex);
            }

            return Task.CompletedTask;
        }

        public virtual void StartDoingTheJob()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInfo("Stopping {0}...".Fill(ServiceName));
                isUpAndRunning = false;
                StopTheJob();
                Logger.LogInfo("Stopped {0}".Fill(ServiceName));
            }
            catch (Exception ex)
            {
            }

            return Task.CompletedTask;
        }

        public virtual void StopTheJob()
        {
            int timeout = 60;
            var stopTimeoutKey = "StopTimeoutInSeconds";
            if (Config.HasSetting(stopTimeoutKey) && Config.GetSetting(stopTimeoutKey).IsNumeric())
            {
                timeout = Config.GetSetting(stopTimeoutKey).ToInt();
            }
            jobThread.Join(TimeSpan.FromSeconds(timeout).Milliseconds);
        }

        public void Dispose()
        {
        }
    }
}
