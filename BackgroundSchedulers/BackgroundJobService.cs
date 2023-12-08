using System;
using System.Collections.Specialized;
using Infrastructure.Cache;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;

namespace BackgroundSchedulers
{
    public class BackgroundJobService : IBackgroundJobService
    {
        #region Private Members        
        private readonly ISchedulerFactory _stdSchedulerFactory;
        private readonly IScheduler _scheduler;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Constructor
        public BackgroundJobService(IServiceProvider serviceProvider, IOptions<AppSettings> options)
        {
            _appSettings = options;
            _serviceProvider = serviceProvider;
            NameValueCollection properties = new NameValueCollection()
            {
                { "quartz.scheduler.instanceName", "QuartzScheduler" },
                { "quartz.scheduler.instanceId", "NON_CLUSTERED" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" },
                { "quartz.jobStore.dataSource", _appSettings.Value.QuartzDataSource },
                { "quartz.dataSource."+_appSettings.Value.QuartzDataSource+".connectionString" , _appSettings.Value.QuartzConnectionString },
                { "quartz.dataSource."+_appSettings.Value.QuartzDataSource+".provider", "SqlServer" },
                { "quartz.jobStore.useProperties", "false" },
                { "quartz.scheduler.idleWaitTime", _appSettings.Value.QuartzIdleTimeWait },
                { "quartz.serializer.type", "json" }
            };
            JobFactory jobFactory = new JobFactory(_serviceProvider);
            _stdSchedulerFactory = new StdSchedulerFactory(properties);
            _scheduler = _stdSchedulerFactory.GetScheduler().Result;
            _scheduler.JobFactory = jobFactory;
            _scheduler.Start().Wait();

        }
        #endregion

        #region Public Methods
        public void ScheduleAutoStopParking(int parkingId, DateTime executeAt)
        {
            IJobDetail jobDetail = JobBuilder.Create<AutoParkingJob>()
                            .WithIdentity(string.Concat(Jobs.AutoStopParking.ToString(), parkingId.ToString()))
                            .UsingJobData(ParkingData.ParkingId.ToString(), parkingId.ToString())
                            .Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(string.Concat(Triggers.AutoStopParking.ToString(), parkingId.ToString()))
                    .ForJob(jobDetail.Key)
                    .StartAt(executeAt.AddSeconds(10))
                    .WithPriority(1)
                    .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void ReScheduleAutoStopParking(int parkingId, DateTime executeAt)
        {
            ITrigger trigger = _scheduler.GetTrigger(new TriggerKey(string.Concat(Triggers.AutoStopParking.ToString(), parkingId.ToString()))).Result;
            IJobDetail jobDetail = _scheduler.GetJobDetail(new JobKey(string.Concat(Jobs.AutoStopParking.ToString(), parkingId.ToString()))).Result;

            ITrigger newTrigger = TriggerBuilder.Create()
                    .WithIdentity(string.Concat(Triggers.AutoStopParking.ToString(), parkingId.ToString()))
                    .ForJob(jobDetail.Key)
                    .StartAt(executeAt.AddSeconds(10))
                    .WithPriority(1)
                    .Build();

            _scheduler.RescheduleJob(trigger.Key, newTrigger);
        }

        public void UnScheduleAutoStopParking(int parkingId)
        {
            _scheduler.UnscheduleJob(new TriggerKey(string.Concat(Triggers.AutoStopParking.ToString(), parkingId.ToString())));
            _scheduler.DeleteJob(new JobKey(string.Concat(Jobs.AutoStopParking.ToString(), parkingId.ToString())));
        }
        #endregion
    }
}