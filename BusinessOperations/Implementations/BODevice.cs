using System;
using System.Collections.Generic;
using System.Linq;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Components.Services.OneSignal;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BODevice : IBODevice
    {
        #region Private Memebers
        private readonly IDeviceService _deviceService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogging _logger;
        private readonly IParkingService _parkingService;
        private readonly IUserSettingService _userSettingService;
        #endregion

        #region Constructor
        public BODevice(IDeviceService deviceService, IPushNotificationService pushNotificationService, ILogging logging, IParkingService parkingService,
            IUserSettingService userSettingService)
        {
            _deviceService = deviceService;
            _pushNotificationService = pushNotificationService;
            _logger = logging;
            _parkingService = parkingService;
            _userSettingService = userSettingService;
        }
        #endregion

        #region Public Methods
        public void Add(Devices devices)
        {
            _deviceService.Add(devices);
        }

        public bool Update(string deviceToken, string registrationToken, string mobileNumber)
        {
            _logger.Debug("Update device process started.");

            bool IsUpdated = false;
            Devices dbDevice = _deviceService.Get(mobileNumber);
            if (dbDevice.IsNotNull())
            {
                dbDevice.UpdatedOn = DateTime.UtcNow;
                dbDevice.DeviceToken = deviceToken;
                dbDevice.RegistrationToken = registrationToken;
                _logger.Debug(string.Format("Updating the user device information with the mobileNumber: {0}", mobileNumber));
                IsUpdated = _deviceService.Update(dbDevice);
                IsUpdated = true;
            }
            else
                throw new Park4YouException(ErrorMessages.DEVICE_NOT_FOUND);

            _logger.Debug("Update device process ended.");

            return IsUpdated;
        }
        #endregion

        #region Private Methods
        private string _GetLogoutPushNotificationMessage()
        {
            return string.Concat("User is logged-In to another device. Please logout.");
        }
        #endregion
    }
}