using System;

namespace Components.Services.OneSignal
{
    public interface IPushNotificationService
    {
        /// <summary>
        /// Cancel the already registered push notification.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        bool CancelNotification(string notificationId);

        /// <summary>
        /// Register the new push notification.
        /// </summary>
        /// <param name="sendTime"></param>
        /// <param name="playerId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        string RegisterPushNotification(DateTime sendTime, string playerId, string message, int parkingId);
    }
}