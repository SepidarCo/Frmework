using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sepidar.Framework
{
    public class Scheduler
    {
        public const string CronExpressionKey = "CronExpression";

        public static void Schedule(Type type, string timing)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add(CronExpressionKey, timing);
            Schedule(type, parameters);
        }

        public static void Schedule(Type type, Dictionary<string, string> parameters)
        {
            var timing = ExtractTiming(parameters);
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();
            string group = GetGroupName(type);
            var jobBuilder = JobBuilder
                .Create(type)
                .WithIdentity(type.Name, group);
            foreach (var parameter in parameters)
            {
                jobBuilder.UsingJobData(parameter.Key, parameter.Value);
            }
            var job = jobBuilder.Build();
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(type.Name, group)
                .StartAt(DateBuilder.NewDate().Build())
                .WithCronSchedule(timing)
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }

        private static string ExtractTiming(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(CronExpressionKey))
            {
                throw new FrameworkException("Missing CronExpression parameter for scheduling");
            }
            var timing = parameters[CronExpressionKey];
            parameters.Remove(CronExpressionKey);
            return timing;
        }

        private static string GetGroupName(Type type)
        {
            return type.Name + "Group";
        }

        public static void Unschedule(Type type)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();
            string group = GetGroupName(type);
            var jobKey = new JobKey(type.Name, group);
            scheduler.Interrupt(jobKey);
        }
    }
}