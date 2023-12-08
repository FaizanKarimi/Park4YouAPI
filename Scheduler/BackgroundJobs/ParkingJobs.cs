using System;
using System.Threading.Tasks;
using Components.Services.Interfaces;
using Quartz;

namespace Scheduler.BackgroundJobs
{
    public class ParkingJobs : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            int parkingId = context.JobDetail.JobDataMap.GetIntValue(ParkingData.ParkingId.ToString());
            string userId = context.JobDetail.JobDataMap.GetString(ParkingData.UserId.ToString());
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            ILogging logger = dataMap[ParkingData.Logger.ToString()] as ILogging;

            logger.Debug($"ParkingId: {parkingId}");
            logger.Debug($"UserId: {userId}");
            logger.Debug($"ExecutedAt: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}