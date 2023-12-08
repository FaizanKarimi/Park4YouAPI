using System;

namespace BackgroundSchedulers
{
    public interface IBackgroundJobService
    {
        /// <summary>
        /// Schedule the auto stop parking.
        /// </summary>
        /// <param name="parkingId"></param>
        /// <param name="userId"></param>
        /// <param name="executeAt"></param>
        /// <param name="stopTime"></param>
        /// <param name="stopParking"></param>
        void ScheduleAutoStopParking(int parkingId, DateTime executeAt);

        /// <summary>
        /// ReSchedule the auto stop parking.
        /// </summary>
        /// <param name="parkingId"></param>
        /// <param name="executeAt"></param>
        void ReScheduleAutoStopParking(int parkingId, DateTime executeAt);

        /// <summary>
        /// UnSchedule the auto stop parking.
        /// </summary>
        /// <param name="parkingId"></param>
        void UnScheduleAutoStopParking(int parkingId);
    }
}