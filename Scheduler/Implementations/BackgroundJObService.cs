using System;
using Components.Services.Interfaces;
using Quartz;
using Quartz.Impl;
using Scheduler.BackgroundJobs;
using Scheduler.Interfaces;

namespace Scheduler.Implementations
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly StdSchedulerFactory _stdSchedulerFactory;
        private readonly IScheduler _scheduler;
        private readonly ILogging _logger;

        public BackgroundJobService(ILogging logging)
        {
            _stdSchedulerFactory = new StdSchedulerFactory();
            _scheduler = _stdSchedulerFactory.GetScheduler().Result;
            _scheduler.Start();
            _logger = logging;
        }

        public void ScheduleAutoStopParking(int parkingId, string userId, DateTime executeAt)
        {
            IJobDetail jobDetail = JobBuilder.Create<ParkingJobs>()
                            .WithIdentity(string.Concat(Jobs.AutoStopParking.ToString(), parkingId))
                            .UsingJobData(ParkingData.ParkingId.ToString(), parkingId)
                            .UsingJobData(ParkingData.UserId.ToString(), userId)
                            .Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(string.Concat(Jobs.AutoStopParking.ToString(), parkingId))
                    .ForJob(jobDetail.Key)
                    .StartAt(executeAt)
                    .Build();

            jobDetail.JobDataMap.Put(ParkingData.Logger.ToString(), _logger);

            _logger.Debug(string.Format("Scheduling the job of autoStop with the parkingId: {0}", parkingId));
            _scheduler.ScheduleJob(jobDetail, trigger);
            _logger.Debug(string.Format("Scheduled the job of autoStop with the parkingId: {0}", parkingId));
        }

        public void UnScheduleAutoStopParking(int parkingId)
        {
            _logger.Debug(string.Format("Unscheduling the job of autoStop with the parkingId: {0}", parkingId));
            _scheduler.UnscheduleJob(new TriggerKey(string.Concat(Triggers.AutoStopParking.ToString(), parkingId)));
            _scheduler.DeleteJob(new JobKey(string.Concat(Jobs.AutoStopParking.ToString(), parkingId)));
            _logger.Debug(string.Format("Unscheduled the job of autoStop with the parkingId: {0}", parkingId));
        }
    }
}