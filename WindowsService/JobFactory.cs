using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Sepidar.WindowsService
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

        }

        public void ReturnJob(IJob job)
        {
            // we let the DI container handler this
        }
    }
}
