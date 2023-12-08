using System;

namespace Scheduler.Interfaces
{
    public interface IBackgroundJobService
    {
        void ScheduleAutoStopParking(int parkingId, string userId, DateTime executeAt);

        void UnScheduleAutoStopParking(int parkingId);
    }
}