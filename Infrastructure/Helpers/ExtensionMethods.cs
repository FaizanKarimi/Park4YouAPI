using System;

namespace Infrastructure.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Check if object is null,
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Check if object is not null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// Get the datetime in formatted format.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}