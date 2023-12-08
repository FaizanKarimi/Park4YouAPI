using System;

namespace Components.Services.Price
{
    public class TimeCalculationService
    {
        /// <summary>
        /// Get total days.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetTotalDays(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime - startTime;
            int hours = Convert.ToInt32(span.Days);
            return hours;
        }

        /// <summary>
        /// Get remaining hours.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetRemainingHours(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime - startTime;
            int hours = Convert.ToInt32(span.Hours);
            return hours;
        }

        /// <summary>
        /// Get remaining minutes.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetRemainingMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime - startTime;
            int totalMinutes = Convert.ToInt32(span.Minutes);
            int minutes = totalMinutes % 60;
            return minutes;
        }

        /// <summary>
        /// Get remaining seconds.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetRemainingSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime - startTime;
            int totalSeconds = Convert.ToInt32(span.Seconds);
            int seconds = totalSeconds % 60;
            return seconds;
        }

        /// <summary>
        /// Get total minutes.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetTotalMinutes(DateTime startTime, DateTime endTime)
        {
            int minutes = 0;
            TimeSpan span = (endTime - startTime);
            minutes = Convert.ToInt32(span.TotalMinutes);
            return minutes;
        }
    }
}