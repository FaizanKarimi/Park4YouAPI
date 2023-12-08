using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Components.Services.Interfaces;
using Components.Services.OneSignal.Models;
using Infrastructure.Cache;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Components.Services.OneSignal
{
    public class PushNotificationService : IPushNotificationService
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public PushNotificationService(IOptions<AppSettings> appSettings, ILogging logging)
        {
            _appSettings = appSettings;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public string RegisterPushNotification(DateTime sendTime, string playerId, string message, int parkingId)
        {
            string json = string.Empty;
            string url = _appSettings.Value.NotificationUrl;
            string appId = _appSettings.Value.NotificationAppId;
            string pushNotificationId = string.Empty;

            _logger.Debug(string.Format("Notification Url is: {0}", url));
            _logger.Debug(string.Format("App Id is: {0}", appId));
            _logger.Debug(string.Format("Sender Id: {0}", playerId));

            var obj = new
            {
                app_id = appId,
                contents = new { en = message },
                headings = new { en = Constants.ParkForYouHeading },
                include_player_ids = new string[] { playerId },
                send_after = sendTime
            };

            string data = JsonConvert.SerializeObject(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.Headers.Clear();
                    client.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                    client.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                    byte[] response = client.UploadData(url, byteArray);
                    _logger.Debug(string.Format("Push is send to one signal"));
                    json = Encoding.Default.GetString(response);
                    _logger.Debug(string.Format("Json is received from the server"));
                }
                RegisterNotification pushNotificationResponse = JsonConvert.DeserializeObject<RegisterNotification>(json);
                _logger.Debug(string.Format("Successfully deserialized the json"));
                pushNotificationId = pushNotificationResponse.id;
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("Exception occurred while sending push notification: {0}", ex));
            }
            return pushNotificationId;
        }

        public bool CancelNotification(string notificationId)
        {
            bool IsCanceled = false;
            string url = _appSettings.Value.NotificationUrl;
            string appId = _appSettings.Value.NotificationAppId;
            try
            {
                url = string.Concat(url, "/", notificationId, "?app_id=", appId);
                using (WebClient client = new WebClient())
                {
                    client.Headers.Clear();
                    client.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                    client.UploadValuesAsync(new Uri(url), HttpMethods.Delete.ToString(), new NameValueCollection());
                    IsCanceled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("Exception occurred while canceling push notification: {0} against notificationId: {1}", ex.Message, notificationId));
            }
            return IsCanceled;
        }
        #endregion

        #region Private Methods
        private string _GetToken()
        {
            return string.Concat(Constants.BasicAuthentication, _appSettings.Value.NotificationAPIKey);
        }
        #endregion
    }
}