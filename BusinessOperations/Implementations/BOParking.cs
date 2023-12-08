using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BackgroundSchedulers;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Components.Services.OneSignal;
using Components.Services.Price;
using Components.Services.PRS;
using Components.Services.QuickPay;
using Components.Services.Solvision;
using Infrastructure.APIResponses.Parking;
using Infrastructure.Cache;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOParking : IBOParking
    {
        #region Private Members
        private readonly IParkingLotService _parkingLotService;
        private readonly IUserCardService _userCardService;
        private readonly IParkingService _parkingService;
        private readonly IVehicleService _vehicleService;
        private readonly IQuickPayService _quickPayService;
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        private readonly IUserSettingService _userSettingService;
        private readonly ILogging _logger;
        private readonly ISolvisionService _solvisionService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDeviceService _deviceService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IPRSService _pRSService;
        #endregion

        #region Constructor
        public BOParking(IParkingLotService parkingLotService, IUserCardService userCardService, IParkingService parkingService, IVehicleService vehicleService,
            IQuickPayService quickPayService, IUserProfileService userProfileService, IEmailService emailService, IPdfService pdfService, IUserSettingService userSettingService,
            ILogging logging, ISolvisionService solvisionService, IPushNotificationService pushNotificationService, IDeviceService deviceService, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, IBackgroundJobService backgroundJobService, IPRSService pRSService)
        {
            _parkingLotService = parkingLotService;
            _userCardService = userCardService;
            _parkingService = parkingService;
            _vehicleService = vehicleService;
            _quickPayService = quickPayService;
            _userProfileService = userProfileService;
            _emailService = emailService;
            _pdfService = pdfService;
            _userSettingService = userSettingService;
            _logger = logging;
            _solvisionService = solvisionService;
            _pushNotificationService = pushNotificationService;
            _deviceService = deviceService;
            _appSettings = options;
            _httpContextAccessor = httpContextAccessor;
            _backgroundJobService = backgroundJobService;
            _pRSService = pRSService;
        }
        #endregion

        #region Public Methods
        public ActiveParkingDetailResponse GetParkingDetails(string registrationNumber)
        {
            _logger.Debug("Active parking details process started.");
            ActiveParkingDetailResponse activeParkingDetailResponse = null;
            _logger.Debug(string.Format("Getting the active parking details against registrationNumber: {0}", registrationNumber));
            Parkings dbParking = _parkingService.GetParkingDetails(registrationNumber);
            if (dbParking.IsNotNull())
            {
                activeParkingDetailResponse = new ActiveParkingDetailResponse()
                {
                    MobileNumber = dbParking.MobileNumber,
                    Price = dbParking.Price,
                    ParkingNote = dbParking.ParkingNote,
                    ParkingLotName = dbParking.Name,
                    StartTime = dbParking.StartTime.ToString(Constants.MobileDateTimeFormat),
                    EndTime = dbParking.StopTime.ToString(Constants.MobileDateTimeFormat),
                    AreaCode = dbParking.AreaCode
                };
            }
            _logger.Debug("Active parking details process ended.");
            return activeParkingDetailResponse;
        }

        public bool SaveParkingNote(int parkingId, string parkingNote)
        {
            _logger.Debug("Parking note process started.");
            bool IsSaved = false;

            Parkings parking = _parkingService.Get(parkingId);
            if (parking.IsNull())
                throw new Park4YouException(ErrorMessages.PARKING_NOT_EXIST);

            IsSaved = _parkingService.SaveParkingNote(parkingId, parkingNote);
            _logger.Debug("Parking note process ended.");
            return IsSaved;
        }

        public StartParkingResponse StartParking(int parkingId, int parkingLotId, int cardId, string mobileNumber, string registrationNumber, string parkingName, int minutes)
        {
            _logger.Debug("Start parking process started.");

            #region Declarations
            StartParkingResponse startParkingResponse = new StartParkingResponse();
            string userId = _httpContextAccessor.GetCurrentUserId();
            decimal calculatedPrice = 0.0M;
            DateTime startTime = DateTime.UtcNow;
            DateTime stopTime = DateTime.UtcNow.AddMinutes(minutes);
            int totalPrice = 0;
            #endregion

            #region Validations
            _logger.Debug(string.Format("Getting device information with the mobileNumber: {0}", mobileNumber));
            Devices device = _deviceService.Get(mobileNumber);
            if (device.IsNull())
                throw new Park4YouException(ErrorMessages.DEVICE_NOT_FOUND);

            _logger.Debug(string.Format("Getting user setting with the mobileNumber: {0}", mobileNumber));
            List<UserSettings> userSettings = _userSettingService.Get(mobileNumber);
            if (userSettings.IsNull() || userSettings.Count == 0)
                throw new Park4YouException(ErrorMessages.USER_SETTING_NOT_EXIST);

            _logger.Debug(string.Format("Getting the user profile information: {0}", mobileNumber));
            UserProfiles userProfile = _userProfileService.Get(mobileNumber);
            if (userProfile.IsNull())
                throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

            _logger.Debug(string.Format("Getting parking lot with the Id: {0}", parkingLotId));
            ParkingLots parkingLot = _parkingLotService.Get(parkingLotId);
            if (parkingLot.IsNull())
                throw new Park4YouException(ErrorMessages.PARKING_LOT_NOT_EXIST);

            int vendor = CommonHelpers.GetVendorIndex(_appSettings.Value.ParkingVendorIds, parkingLot.UserId);
            _logger.Debug(string.Format("Parking vendor index is: {0}", vendor));

            _logger.Debug(string.Format("Getting user card information with the cardId: {0}", cardId));
            UserCards userCard = _userCardService.Get(cardId);
            if (userCard.IsNull())
                throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);

            _logger.Debug(string.Format("Getting vehicle information with the mobileNumber: {0}", mobileNumber));
            Vehicles vehicle = _vehicleService.Get(mobileNumber);
            if (vehicle.IsNull())
                throw new Park4YouException(ErrorMessages.VEHICLE_DOES_NOT_EXIST);
            #endregion

            _logger.Debug(string.Format("Getting parking information with the parkingId: {0}", parkingId));
            Parkings dbParking = _parkingService.Get(parkingId);

            if (dbParking.IsNotNull())
            {
                #region Extend Parking
                startTime = dbParking.StartTime;
                stopTime = dbParking.StopTime.AddMinutes(minutes);
                dbParking.StopTime = stopTime;
                minutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));

                _logger.Debug(string.Format("Calculated price of minutes {0} is {1}: ", minutes, calculatedPrice));
                totalPrice = Convert.ToInt32(Convert.ToDouble(parkingLot.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue));
                _logger.Debug(string.Format("The total price is of: {0}", totalPrice));

                _logger.Debug(string.Format("Calculating the price with the minutes of: {0}", minutes));
                calculatedPrice = PriceService.CalculatePrice(startTime, stopTime, parkingLot.ChargeSheetPrices);

                #region PRS(KK)
                if (vendor == (int)Vendors.PRS)
                {
                    string transactionId = CommonHelpers.GetOrderId();
                    _logger.Debug(string.Format("Registering the parking on prs(KK) with the areaCode: {0} and parkingLotId: {1}", parkingLot.AreaCode, parkingLot.Id));
                    _pRSService.UpdateParking(dbParking.VendorParkingId, transactionId, parkingLot.Name, parkingLot.Name, parkingLot.CenterCoordinates, "KK", "BI", vehicle.RegistrationNumber.ToString(), startTime, stopTime, "0-1", false);
                }
                #endregion

                dbParking.Price = calculatedPrice;
                dbParking.CardId = cardId;

                #region Push Notification
                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId15Min))
                    _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId15Min);

                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId30Min))
                    _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId30Min);

                bool pushNotification15Minutes = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES.ToString()))?.AttributeValue);
                bool pushNotification30Minutes = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES.ToString()))?.AttributeValue);
                if (pushNotification15Minutes && dbParking.StopTime >= DateTime.UtcNow.AddMinutes(15))
                {
                    DateTime sendAt = dbParking.StopTime.AddMinutes(-15);
                    _logger.Debug(string.Format("Register push notification with the time frame less than fivteen minutes."));
                    dbParking.ParkingNotifications.NotificationId15Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(dbParking), dbParking.Id);
                }

                if (pushNotification30Minutes && dbParking.StopTime >= DateTime.UtcNow.AddMinutes(30))
                {
                    DateTime sendAt = dbParking.StopTime.AddMinutes(-30);
                    _logger.Debug(string.Format("Register push notification with the time frame less than thiry minutes."));
                    dbParking.ParkingNotifications.NotificationId30Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(dbParking), dbParking.Id);
                }
                #endregion

                _logger.Debug(string.Format("Updating the existing parking with the Id: {0}", parkingId));
                dbParking = _parkingService.Update(dbParking, dbParking.ParkingNotifications);
                #endregion                                

                #region ReSchedule AutoStopParking
                _logger.Debug(string.Format("Background service register event."));
                int totalMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
                DateTime jobTime = DateTime.Now.AddMinutes(totalMinutes);
                _backgroundJobService.ReScheduleAutoStopParking(dbParking.Id, jobTime);
                #endregion
            }
            else
            {
                #region New Parking
                Parkings newParking = new Parkings();

                totalPrice = Convert.ToInt32(Convert.ToDouble(parkingLot.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue));
                _logger.Debug(string.Format("Parking total price is of: {0}", totalPrice));

                calculatedPrice = PriceService.CalculatePrice(startTime, stopTime, parkingLot.ChargeSheetPrices);
                _logger.Debug(string.Format("Calculated price of minutes {0} is {1}: ", minutes, calculatedPrice));

                #region Solvision / PRS(KK)
                if (vendor == (int)Vendors.SOLVISION)
                {
                    _logger.Debug(string.Format("Registering the parking on solvision with the areaCode: {0} and parkingLotId: {1}", parkingLot.AreaCode, parkingLot.Id));
                    _solvisionService.StartParking(parkingName, parkingLot.AreaCode, registrationNumber, startTime);
                }
                else if (vendor == (int)Vendors.PRS)
                {
                    string transactionId = CommonHelpers.GetOrderId();
                    _logger.Debug(string.Format("Registering the parking on prs(KK) with the areaCode: {0} and parkingLotId: {1}", parkingLot.AreaCode, parkingLot.Id));
                    newParking.VendorParkingId = _pRSService.StartParking(transactionId, parkingLot.Name, parkingLot.Name, parkingLot.CenterCoordinates, "KK", "BI", vehicle.RegistrationNumber.ToString(), startTime, stopTime, "0-1", false);
                }
                #endregion

                newParking.AreaCode = parkingLot.AreaCode;
                newParking.IsFixed = true;
                newParking.IsLatest = true;
                newParking.MobileNumber = mobileNumber;
                newParking.ParkingLotId = parkingLotId;
                newParking.RegistrationNumber = registrationNumber;
                newParking.Name = parkingName;
                newParking.ParkingNote = string.Empty;
                newParking.ParkingStatus = ParkingStatus.START.ToString();
                newParking.ParkingStatusId = (int)ParkingStatus.START;
                newParking.StartTime = startTime;
                newParking.StopTime = stopTime;
                newParking.VehicleId = vehicle.Id;
                newParking.Price = calculatedPrice;
                newParking.TotalPrice = totalPrice;
                newParking.CardId = cardId;

                #region Push Notification
                ParkingNotifications parkingNotifications = new ParkingNotifications();

                bool pushNotification15Minutes = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES.ToString()))?.AttributeValue);
                bool pushNotification30Minutes = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES.ToString()))?.AttributeValue);
                if (pushNotification15Minutes && newParking.StopTime >= DateTime.UtcNow.AddMinutes(15))
                {
                    DateTime sendAt = newParking.StopTime.AddMinutes(-15);
                    parkingNotifications.NotificationId15Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(newParking), 0);
                }

                if (pushNotification30Minutes && newParking.StopTime >= DateTime.UtcNow.AddMinutes(30))
                {
                    DateTime sendAt = newParking.StopTime.AddMinutes(-30);
                    parkingNotifications.NotificationId30Min = _pushNotificationService.RegisterPushNotification(sendAt, device.RegistrationToken, _parkingService.GetParkingPushNotificationMessage(newParking), 0);
                }
                #endregion

                _logger.Debug(string.Format("Adding the new parking in the system."));
                dbParking = _parkingService.StartParking(newParking, parkingNotifications);
                #endregion

                #region Auto Stop Scheduler
                _logger.Debug(string.Format("Background service register event."));
                int totalMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
                DateTime jobTime = DateTime.Now.AddMinutes(totalMinutes);
                _backgroundJobService.ScheduleAutoStopParking(dbParking.Id, jobTime);
                #endregion
            }

            #region Response Mapping
            startParkingResponse.ParkingId = dbParking.Id;
            startParkingResponse.ParkingLotId = dbParking.ParkingLotId;
            startParkingResponse.AreaCode = dbParking.AreaCode;
            startParkingResponse.ParkingName = dbParking.Name;
            startParkingResponse.RegistrationNumber = dbParking.RegistrationNumber;
            startParkingResponse.RemainingMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, DateTime.UtcNow));
            startParkingResponse.TotalMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
            startParkingResponse.TotalPrice = CommonHelpers.ConvertedAmount(dbParking.Price, userProfile.Country);
            startParkingResponse.BasePrice = totalPrice;
            startParkingResponse.StartTime = dbParking.StartTime.ToString(Constants.MobileDateTimeFormat);
            startParkingResponse.StopTime = dbParking.StopTime.ToString(Constants.MobileDateTimeFormat);
            startParkingResponse.VehicleName = vehicle.Name;
            startParkingResponse.CenterPoint = parkingLot.CenterCoordinates;
            startParkingResponse.IsFixed = Convert.ToBoolean(dbParking.IsFixed);
            #endregion

            _logger.Debug("Start parking process ended.");

            return startParkingResponse;
        }

        public StopParkingResponse StopParking(int parkingId, bool isAutoStopFlow, int remainingSeconds)
        {
            _logger.Debug("Stop parking process started.");

            StopParkingResponse stopParkingResponse = new StopParkingResponse();
            decimal calculatedPrice = 0.0M;

            #region UnSchedule Auto Stop Parking
            _logger.Debug(string.Format("Going to unschedule the auto stop parking from the scheduler with the parkingId: {0}", parkingId));
            _backgroundJobService.UnScheduleAutoStopParking(parkingId);
            #endregion

            #region Validations
            _logger.Debug(string.Format("Getting parking with the parkingId: {0}", parkingId));
            Parkings dbParking = _parkingService.GetStartParking(parkingId);
            if (dbParking.IsNull())
                throw new Park4YouException(ErrorMessages.PARKING_ALREADY_ENDED);

            dbParking.StopTime = dbParking.StopTime.AddSeconds(-remainingSeconds);

            int minutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
            int seconds = CommonHelpers.GetSecondsDifference(dbParking.StopTime, dbParking.StartTime);

            _logger.Debug(string.Format("Minutes is: {0}", minutes));
            _logger.Debug(string.Format("Seconds is: {0}", seconds));

            _logger.Debug(string.Format("Now the stop time is: {0}", dbParking.StopTime));

            _logger.Debug(string.Format("Getting the parking lot with the parkingLotId: {0}", dbParking.ParkingLotId));
            ParkingLots parkingLot = _parkingLotService.Get(dbParking.ParkingLotId);
            if (parkingLot.IsNull())
                throw new Park4YouException(ErrorMessages.PARKING_LOT_NOT_EXIST);

            int vendor = CommonHelpers.GetVendorIndex(_appSettings.Value.ParkingVendorIds, parkingLot.UserId);
            _logger.Debug(string.Format("Parking vendor index is: {0}", vendor));

            _logger.Debug(string.Format("Getting the user card information with the cardId: {0}", dbParking.CardId));
            UserCards userCard = _userCardService.Get(dbParking.CardId);
            if (userCard.IsNull())
                throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);

            _logger.Debug(string.Format("Getting the user setting with the mobileNumber: {0}", dbParking.MobileNumber));
            List<UserSettings> userSettings = _userSettingService.Get(dbParking.MobileNumber);
            if (userSettings.IsNull() || userSettings.Count == 0)
                throw new Park4YouException(ErrorMessages.USER_SETTING_NOT_EXIST);

            _logger.Debug(string.Format("Getting user profile infomation with the mobileNumber: {0}", dbParking.MobileNumber));
            UserProfiles userProfile = _userProfileService.Get(dbParking.MobileNumber);
            if (userProfile.IsNull())
                throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

            _logger.Debug(string.Format("Getting the device information with the mobileNumber: {0}", dbParking.MobileNumber));
            Devices device = _deviceService.Get(dbParking.MobileNumber);
            if (device.IsNull())
                throw new Park4YouException(ErrorMessages.DEVICE_NOT_FOUND);
            #endregion

            #region Price Calculation
            int totalPrice = Convert.ToInt32(Convert.ToDouble(parkingLot.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue));

            if (!isAutoStopFlow)
            {
                var diff = dbParking.StopTime - dbParking.StartTime;
                int second = 60 - diff.Seconds;
                dbParking.StopTime = dbParking.StopTime.AddSeconds(second);
            }

            _logger.Debug(string.Format("Stop Time: {0}", dbParking.StopTime));
            int totalSeconds = CommonHelpers.GetTotalSeconds(dbParking.StopTime, dbParking.StartTime);
            _logger.Debug(string.Format("Total seconds is: {0}", totalSeconds));

            if (totalSeconds > 180)
            {
                _logger.Debug(string.Format("Calculating the price with the minutes of: {0}", minutes));
                calculatedPrice = PriceService.CalculatePrice(dbParking.StartTime, dbParking.StopTime, parkingLot.ChargeSheetPrices);
                _logger.Debug(string.Format("The calculated price is: {0}", calculatedPrice));
            }
            #endregion

            #region Payment
            string expirationMonth = CommonHelpers.GetCardExpirationMonth(userCard.CardExpiry);
            string expirationYear = CommonHelpers.GetCardExpirationYear(userCard.CardExpiry);

            _logger.Debug(string.Format("Going to deposit the amount with the cardNumber: {0}", userCard.CardNumber));
            PaymentOrders paymentOrder = _quickPayService.ParkingPayment(calculatedPrice.ToString(), expirationMonth, expirationYear, userCard.CardVerficationValue, userCard.CardNumber);
            if (paymentOrder.IsNotNull())
            {
                paymentOrder.ParkingId = parkingId;
            }
            #endregion

            #region Stop Parking
            dbParking.Price = calculatedPrice;
            dbParking.ParkingStatus = ParkingStatus.STOP.ToString();
            dbParking.ParkingStatusId = (int)ParkingStatus.STOP;

            _logger.Debug(string.Format("Updating the parking status to stop with the parkingId: {0}", parkingId));
            _parkingService.StopParking(dbParking, paymentOrder);

            _logger.Debug(string.Format("Getting the parking with the parkingId: {0}", parkingId));
            dbParking = _parkingService.Get(parkingId);
            #endregion

            #region Solvision / PRS(KK)
            if (vendor == (int)Vendors.SOLVISION)
            {
                _logger.Debug(string.Format("Going to stop the parking at solvision."));
                _solvisionService.StopParking(dbParking.Name, parkingLot.AreaCode, dbParking.RegistrationNumber, dbParking.StopTime);
            }
            else if (vendor == (int)Vendors.PRS)
            {
                string transactionId = CommonHelpers.GetOrderId();
                _logger.Debug(string.Format("Going to stop the parking at prs."));
                _pRSService.UpdateParking(dbParking.VendorParkingId, transactionId, parkingLot.Name, parkingLot.Name, parkingLot.CenterCoordinates, "KK", "BI", dbParking.RegistrationNumber, dbParking.StartTime, dbParking.StopTime, "0-1", false);
            }
            #endregion

            #region Cancel Push Notification
            if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId15Min))
            {
                _logger.Debug(string.Format("Going to cancel the push notification of 15 minutes with the notificationId: {0}", dbParking.ParkingNotifications.NotificationId15Min));
                _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId15Min);
            }

            if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId30Min))
            {
                _logger.Debug(string.Format("Going to cancel the push notification of 30 minutes with the notificationId: {0}", dbParking.ParkingNotifications.NotificationId30Min));
                _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId30Min);
            }

            _pushNotificationService.RegisterPushNotification(DateTime.UtcNow, device.RegistrationToken, "You parking has been finished", parkingId);
            #endregion

            #region Sending Email
            bool IsSendEmail = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_PDF_RECEIPT.ToString()))?.AttributeValue);
            if (IsSendEmail)
            {
                _logger.Debug(string.Format("Goint to send the parking receipt with the parkingId: {0}", parkingId));
                this._SendReceipt(dbParking.MobileNumber, parkingId);
            }
            #endregion

            #region Response Mapping
            stopParkingResponse.ParkingId = dbParking.Id;
            stopParkingResponse.ParkingLotId = dbParking.ParkingLotId;
            stopParkingResponse.AreaCode = dbParking.AreaCode;
            stopParkingResponse.ParkingName = dbParking.Name;
            stopParkingResponse.RegistrationNumber = dbParking.RegistrationNumber;
            stopParkingResponse.RemainingMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StopTime));
            stopParkingResponse.TotalMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
            stopParkingResponse.TotalPrice = CommonHelpers.ConvertedAmount(dbParking.Price, userProfile.Country);
            stopParkingResponse.BasePrice = totalPrice;
            stopParkingResponse.StartTime = dbParking.StartTime.ToString(Constants.MobileDateTimeFormat);
            stopParkingResponse.StopTime = dbParking.StopTime.ToString(Constants.MobileDateTimeFormat);
            stopParkingResponse.CenterPoint = parkingLot.CenterCoordinates;
            stopParkingResponse.IsFixed = Convert.ToBoolean(dbParking.IsFixed);
            #endregion

            _logger.Debug("Stop parking process ended.");

            return stopParkingResponse;
        }

        public bool SendReceipt(string mobileNumber, int parkingId)
        {
            _logger.Debug("Send receipt process started.");
            bool IsSend = false;

            #region Validation
            _logger.Debug(string.Format("Getting the parking with the parkingId: {0}", parkingId));
            Parkings parking = _parkingService.Get(parkingId);
            if (parking.IsNull())
                throw new Park4YouException(ErrorMessages.PARKING_NOT_EXIST);

            _logger.Debug(string.Format("Getting the user profile information with the mobileNumber: {0}", mobileNumber));
            UserProfiles userProfile = _userProfileService.Get(mobileNumber);
            if (userProfile.IsNull())
                throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

            if (string.IsNullOrEmpty(userProfile.EmailAddress))
                throw new Park4YouException(ErrorMessages.USER_EMAIL_NOT_FOUND);
            #endregion

            _logger.Debug(string.Format("Getting the parking report with the parkingId: {0}", parkingId));
            ParkingReport parkingReport = _parkingService.GetParkingReport(parkingId);

            string billingHtml = _GetBillRecieptHtml();
            string orderId = CommonHelpers.GetOrderId();
            string userName = string.Concat(userProfile.FirstName, " ", userProfile.LastName);
            string fileName = string.Concat("receipt-", parkingId, "-", orderId, "-", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, ".pdf");
            string recieptId = string.Concat(parkingId, "-", orderId);
            string html = _GetParkingReportConvertedHtml(billingHtml, parkingReport, userProfile);

            _logger.Debug(string.Format("Going to convert the parking report to pdf with the parkingId:{0}", parkingId));
            bool IsConverted = _pdfService.ConvertParkingReport(html, fileName);
            if (IsConverted)
            {
                _logger.Debug(string.Format("Going to send the parking receipt to the user."));
                _emailService.SendParkingReceiptEmail(userName, userProfile.EmailAddress, recieptId, fileName);
                IsSend = true;
            }

            _logger.Debug("Send receipt process ended.");

            return IsSend;
        }

        public List<ParkingHistoryResponse> GetParkingHistory(string mobileNumber)
        {
            _logger.Debug("Parking history process started.");
            List<ParkingHistoryResponse> parkingHistoryResponses = new List<ParkingHistoryResponse>();

            _logger.Debug(string.Format("Getting the parking history with the mobileNumber: {0}", mobileNumber));
            List<ParkingHistory> parkingHistory = _parkingService.GetParkingHistory(mobileNumber);

            _logger.Debug(string.Format("Getting the user profile information with the mobileNumber: {0}", mobileNumber));
            UserProfiles userProfile = _userProfileService.Get(mobileNumber);

            parkingHistory.ForEach((parking) =>
            {
                bool Exist = parkingHistoryResponses.Exists(x => x.ParkingId == parking.ParkingId);
                if (!Exist)
                {
                    ParkingHistoryResponse parkingHistoryResponse = new ParkingHistoryResponse()
                    {
                        AreaCode = parking.AreaCode,
                        Name = parking.Name,
                        Note = parking.Note,
                        ParkingId = parking.ParkingId,
                        TotalPrice = CommonHelpers.ConvertedAmount(parking.Price, userProfile?.Country),
                        RegistrationNumber = parking.RegistrationNumber,
                        RemainingMinutes = parking.RemainingMinutes,
                        StartTime = parking.StartTime.ToString(Constants.MobileDateTimeFormat),
                        StopTime = parking.StopTime.ToString(Constants.MobileDateTimeFormat),
                        VehicleName = parking.VehicleName
                    };
                    parkingHistoryResponses.Add(parkingHistoryResponse);
                }
            });

            _logger.Debug("Parking history process ended.");

            return parkingHistoryResponses;
        }

        public List<ParkingLotModel> GetParkingAroundAreas(string latitude, string longitude)
        {
            _logger.Debug("Parking around areas process started.");
            string userId = _httpContextAccessor.GetCurrentUserId();

            List<ParkingLotModel> parkingLotModelList = new List<ParkingLotModel>();

            _logger.Debug(string.Format("Getting the parking around areas with the latitude: {0} and longitude: {0}", latitude, longitude));
            List<ParkingAroundAreas> parkingAroundAreas = _parkingService.GetParkingAroundAreas(latitude, longitude);

            _logger.Debug(string.Format("Getting the user profile information with the userId: {0}", userId));
            UserProfiles userProfile = _userProfileService.GetByUserId(userId);
            _logger.Debug(string.Format("User profile information with the userId is fetched: {0}", userId));

            parkingAroundAreas.ForEach((parkingLot) =>
            {
                decimal convertedPrice = Convert.ToDecimal(CommonHelpers.FormattedPriceToDecimal(parkingLot.BasePrice));
                ParkingLotModel parkingLotModel = new ParkingLotModel();
                List<LongitudeLatitudeModel> longitudeLatitudeModelList = new List<LongitudeLatitudeModel>();
                parkingLotModel.AreaCode = parkingLot.AreaCode;
                parkingLotModel.CenterPoint = parkingLot.CenterCoordinates;
                parkingLotModel.Name = parkingLot.Name;
                parkingLotModel.ParkingLotId = parkingLot.ParkingLotId;
                parkingLotModel.BasePrice = convertedPrice == 0.0m ? "Free" : CommonHelpers.ConvertedAmount(convertedPrice, userProfile?.Country);
                parkingLotModel.Distance = parkingLot.Distance;
                if (parkingLot.AreaGeoCoordinates != null)
                {
                    string geoCoordinates = parkingLot.AreaGeoCoordinates.TrimEnd();
                    string[] geoCoordinatesArray = geoCoordinates.Split(' ');
                    for (int i = 0; i < geoCoordinatesArray.Length; i++)
                    {
                        LongitudeLatitudeModel longitudeLatitudeModel = new LongitudeLatitudeModel();
                        string areas = geoCoordinatesArray[i];
                        string[] ss = areas.Split(',');
                        longitudeLatitudeModel.Latitude = ss[0].ToString();
                        longitudeLatitudeModel.Longitude = ss[1].ToString();
                        longitudeLatitudeModelList.Add(longitudeLatitudeModel);
                    }
                }
                parkingLotModel.AreaGeoCoordinates = longitudeLatitudeModelList.ToList();
                parkingLotModelList.Add(parkingLotModel);
            });

            _logger.Debug("Parking around areas process ended.");

            return parkingLotModelList;
        }

        public void CancelStartedParkingPushNotifications()
        {
            _logger.Debug("Cancel push notification process started");
            string userId = _httpContextAccessor.GetCurrentUserId();
            _logger.Debug(string.Format("Getting started parking with userId: {0}", userId));
            Parkings dbParking = _parkingService.GetStartedParking(userId);
            if (dbParking.IsNotNull())
            {
                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId15Min))
                {
                    _logger.Debug(string.Format("Going to cancel push notification for parking reminder with 15 minutes remaining: {0}", dbParking.ParkingNotifications.NotificationId15Min));
                    _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId15Min);
                }

                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId30Min))
                {
                    _logger.Debug(string.Format("Going to cancel push notification for parking reminder with 30 minutes remaining: {0}", dbParking.ParkingNotifications.NotificationId30Min));
                    _pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId30Min);
                }
            }
            _logger.Debug("Cancel push notification process ended");
        }
        #endregion

        #region Private Methods
        private string _GetBillRecieptHtml()
        {
            var baseDirectory = CommonHelpers.GetDomainName();
            var filePath = string.Concat(baseDirectory, @"\HtmlTemplates\BillReciept.html");
            string text = File.ReadAllText(filePath);
            return text;
        }

        private string _GetParkingReportConvertedHtml(string html, ParkingReport parkingReport, UserProfiles userProfile)
        {
            StringBuilder info = new StringBuilder(html);
            info.Replace("_OrderID_", parkingReport.OrderId);
            info.Replace("_FirstName_", userProfile.FirstName);
            info.Replace("_LastName_", userProfile.LastName);
            info.Replace("_BIN_", parkingReport.Bin);
            info.Replace("_Last4_", parkingReport.Last4);
            info.Replace("_Country_", userProfile.Country);
            info.Replace("_Brand_", parkingReport.Brand);
            info.Replace("_Town_", userProfile.Town);
            info.Replace("_QPStatusMessage_", parkingReport.QPStatusMessage);
            info.Replace("_Price_", parkingReport.Price.ToString());
            info.Replace("_Name_", parkingReport.Name);
            info.Replace("_AreaCode_", parkingReport.AreaCode);
            info.Replace("_RegistrationNumber_", parkingReport.RegistrationNumber);
            info.Replace("_StartTime_", string.Concat(parkingReport.StartTime.GetDateTimeFormat(), " ", "UTC"));
            info.Replace("_StopTime_", string.Concat(parkingReport.StopTime.GetDateTimeFormat(), " ", "UTC"));
            info.Replace("_Note_", parkingReport.ParkingNote);
            return info.ToString();
        }

        private bool _SendReceipt(string mobileNumber, int parkingId)
        {
            bool IsSend = false;

            _logger.Debug(string.Format("Getting the user profile information with the mobileNumber: {0}", mobileNumber));
            UserProfiles userProfile = _userProfileService.Get(mobileNumber);
            if (userProfile.IsNull())
                throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

            if (string.IsNullOrEmpty(userProfile.EmailAddress))
                throw new Park4YouException(ErrorMessages.USER_EMAIL_NOT_FOUND);

            _logger.Debug(string.Format("Getting the parking report with the parkingId: {0}", parkingId));
            ParkingReport parkingReport = _parkingService.GetParkingReport(parkingId);

            string billingHtml = _GetBillRecieptHtml();
            string orderId = CommonHelpers.GetOrderId();
            string userName = string.Concat(userProfile.FirstName, " ", userProfile.LastName);
            string fileName = string.Concat("receipt-", parkingId, "-", orderId, "-", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, ".pdf");
            string recieptId = string.Concat(parkingId, "-", orderId);
            string html = _GetParkingReportConvertedHtml(billingHtml, parkingReport, userProfile);

            _logger.Debug(string.Format("Going to convert the parking report to pdf with the parkingId:{0}", parkingId));
            bool IsConverted = _pdfService.ConvertParkingReport(html, fileName);
            if (IsConverted)
            {
                _emailService.SendParkingReceiptEmail(userName, userProfile.EmailAddress, recieptId, fileName);
                IsSend = true;
            }

            return IsSend;
        }
        #endregion
    }
}