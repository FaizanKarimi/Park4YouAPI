using System;
using BusinessOperations.Interfaces;
using Components.Services.OneSignal;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOUserSetting : IBOUserSetting
    {
        #region Private Members
        private readonly IUserSettingService _userSettingService;
        private readonly IParkingService _parkingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDeviceService _deviceService;
        #endregion

        #region Constructor
        public BOUserSetting(IUserSettingService userSettingService, IParkingService parkingService, IHttpContextAccessor httpContextAccessor,
            IPushNotificationService pushNotificationService, IDeviceService deviceService)
        {
            _userSettingService = userSettingService;
            _parkingService = parkingService;
            _httpContextAccessor = httpContextAccessor;
            _pushNotificationService = pushNotificationService;
            _deviceService = deviceService;
        }
        #endregion

        #region Public Methods
        public bool Update(UserSettings userSetting)
        {
            #region Validation
            try
            {
                CommonHelpers.ParseEnum<UserSetting>(userSetting.AttributeKey);
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.USER_SETTING_INCORRECT_ATTRIBUTE_KEY);
            }
            #endregion

            #region Cancel / Register Parking Push Notification
            bool PushNotificationSettingExist = CommonHelpers.TryParseEnum<ParkingPushNotificationSettings>(userSetting.AttributeKey);
            if (PushNotificationSettingExist)
            {
                bool IsPushNotificationSettingOn = Convert.ToBoolean(userSetting.AttributeValue);
                if (!IsPushNotificationSettingOn)
                {
                    string userId = _httpContextAccessor.GetCurrentUserId();
                    Parkings dbParking = _parkingService.GetStartedParking(userId);
                    if (dbParking.IsNotNull())
                    {
                        UserSetting pushNotificationSetting = CommonHelpers.ParseEnum<UserSetting>(userSetting.AttributeKey);
                        string pushNotificationId = _GetParkingPushNotificationId(pushNotificationSetting, dbParking.ParkingNotifications);
                        if (!string.IsNullOrEmpty(pushNotificationId))
                            _pushNotificationService.CancelNotification(pushNotificationId);
                    }
                }
                else
                {
                    string userId = _httpContextAccessor.GetCurrentUserId();
                    Parkings dbParking = _parkingService.GetStartedParking(userId);
                    Devices device = _deviceService.GetByUserId(userId);
                    if (dbParking.IsNotNull())
                    {
                        UserSetting pushNotificationSetting = CommonHelpers.ParseEnum<UserSetting>(userSetting.AttributeKey);
                        if (pushNotificationSetting == UserSetting.PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES && dbParking.StopTime >= DateTime.UtcNow.AddMinutes(15))
                        {
                            DateTime sendAt = dbParking.StopTime.AddMinutes(-15);
                            dbParking.ParkingNotifications.NotificationId15Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(dbParking), 0);
                        }

                        if (pushNotificationSetting == UserSetting.PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES && dbParking.StopTime >= DateTime.UtcNow.AddMinutes(30))
                        {
                            DateTime sendAt = dbParking.StopTime.AddMinutes(-30);
                            dbParking.ParkingNotifications.NotificationId30Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(dbParking), 0);
                        }

                        _parkingService.Update(dbParking, dbParking.ParkingNotifications);
                    }
                }
            }
            #endregion

            return _userSettingService.Update(userSetting);
        }
        #endregion

        #region Private Methods
        private string _GetParkingPushNotificationId(UserSetting pushNotificationSetting, ParkingNotifications parkingNotifications)
        {
            string pushNotificationId = string.Empty;
            switch (pushNotificationSetting)
            {
                case UserSetting.PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES:
                    pushNotificationId = parkingNotifications.NotificationId15Min;
                    break;
                case UserSetting.PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES:
                    pushNotificationId = parkingNotifications.NotificationId30Min;
                    break;
                default:
                    //Do nothing.
                    break;
            }
            return pushNotificationId;
        }
        #endregion
    }
}