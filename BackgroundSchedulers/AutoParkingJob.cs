using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components.Services.Interfaces;
using Components.Services.OneSignal;
using Components.Services.Price;
using Components.Services.PRS;
using Components.Services.QuickPay;
using Components.Services.Solvision;
using Infrastructure.Cache;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Services.Interfaces;

namespace BackgroundSchedulers
{
    [DisallowConcurrentExecution]
    public class AutoParkingJob : IJob
    {
        public IServiceProvider Container { get; }

        public AutoParkingJob(IServiceProvider container)
        {
            Container = container;
        }

        public Task Execute(IJobExecutionContext context)
        {
            IParkingService parkingService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IParkingService)) as IParkingService;
            IParkingLotService parkingLotService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IParkingLotService)) as IParkingLotService;
            IUserCardService userCardService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IUserCardService)) as IUserCardService;
            IUserSettingService userSettingService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IUserSettingService)) as IUserSettingService;
            IUserProfileService userProfileService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IUserProfileService)) as IUserProfileService;
            IDeviceService deviceService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IDeviceService)) as IDeviceService;
            IQuickPayService quickPayService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IQuickPayService)) as IQuickPayService;
            IPushNotificationService pushNotificationService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IPushNotificationService)) as IPushNotificationService;
            ISolvisionService solvisionService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(ISolvisionService)) as ISolvisionService;
            IPRSService pRSService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IPRSService)) as IPRSService;
            ILogging logger = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(ILogging)) as ILogging;
            IPdfService pdfService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IPdfService)) as IPdfService;
            IEmailService emailService = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IEmailService)) as IEmailService;
            IOptions<AppSettings> appSettings = Container.CreateScope().ServiceProvider.GetRequiredService(typeof(IOptions<AppSettings>)) as IOptions<AppSettings>;
            int parkingId = Convert.ToInt32(context.JobDetail.JobDataMap.GetString(ParkingData.ParkingId.ToString()));

            logger.Debug("Auto stop parking process started.");

            bool IsStopped = false;
            decimal calculatedPrice = 0.0M;

            logger.Debug(string.Format("Getting parking with the parkingId: {0}", parkingId));
            Parkings dbParking = parkingService.GetStartParking(parkingId);
            if (dbParking.IsNotNull())
            {
                #region Validations
                logger.Debug(string.Format("Getting the parking lot with the parkingLotId: {0}", dbParking.ParkingLotId));
                ParkingLots parkingLot = parkingLotService.Get(dbParking.ParkingLotId);
                if (parkingLot.IsNull())
                    throw new Park4YouException(ErrorMessages.PARKING_LOT_NOT_EXIST);

                int vendor = CommonHelpers.GetVendorIndex(appSettings.Value.ParkingVendorIds, parkingLot.UserId);
                logger.Debug(string.Format("Parking vendor index is: {0}", vendor));

                logger.Debug(string.Format("Getting the user card information with the cardId: {0}", dbParking.CardId));
                UserCards userCard = userCardService.Get(dbParking.CardId);
                if (userCard.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);

                logger.Debug(string.Format("Getting the user setting with the mobileNumber: {0}", dbParking.MobileNumber));
                List<UserSettings> userSettings = userSettingService.Get(dbParking.MobileNumber);
                if (userSettings.IsNull() || userSettings.Count == 0)
                    throw new Park4YouException(ErrorMessages.USER_SETTING_NOT_EXIST);

                logger.Debug(string.Format("Getting user profile infomation with the mobileNumber: {0}", dbParking.MobileNumber));
                UserProfiles userProfile = userProfileService.Get(dbParking.MobileNumber);
                if (userProfile.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                logger.Debug(string.Format("Getting the device information with the mobileNumber: {0}", dbParking.MobileNumber));
                Devices device = deviceService.Get(dbParking.MobileNumber);
                if (device.IsNull())
                    throw new Park4YouException(ErrorMessages.DEVICE_NOT_FOUND);
                #endregion

                int minutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(dbParking.StopTime, dbParking.StartTime));
                int seconds = CommonHelpers.GetSecondsDifference(dbParking.StopTime, dbParking.StartTime);

                logger.Debug(string.Format("Stop Time: {0}", dbParking.StopTime));
                int totalSeconds = CommonHelpers.GetTotalSeconds(dbParking.StopTime, dbParking.StartTime);
                logger.Debug(string.Format("Total seconds is: {0}", totalSeconds));

                if (totalSeconds > 180)
                {
                    logger.Debug(string.Format("Calculating the price with the seconds of: {0}", seconds));
                    calculatedPrice = PriceService.CalculatePrice(dbParking.StartTime, dbParking.StopTime, parkingLot.ChargeSheetPrices);
                    logger.Debug(string.Format("Calculated the price with the minutes of: {0}", minutes));
                }

                int totalPrice = Convert.ToInt32(Convert.ToDouble(parkingLot.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue));

                string expirationMonth = CommonHelpers.GetCardExpirationMonth(userCard.CardExpiry);
                string expirationYear = CommonHelpers.GetCardExpirationYear(userCard.CardExpiry);

                logger.Debug(string.Format("Going to deposit the amount with the cardNumber: {0}", userCard.CardNumber));
                PaymentOrders paymentOrder = quickPayService.ParkingPayment(calculatedPrice.ToString(), expirationMonth, expirationYear, userCard.CardVerficationValue, userCard.CardNumber);
                paymentOrder.ParkingId = parkingId;

                dbParking.Price = calculatedPrice;
                dbParking.ParkingStatus = ParkingStatus.STOP.ToString();
                dbParking.ParkingStatusId = (int)ParkingStatus.STOP;

                logger.Debug(string.Format("Updating the parking status to stop with the parkingId: {0}", parkingId));
                IsStopped = parkingService.StopParking(dbParking, paymentOrder);

                logger.Debug(string.Format("Getting the parking with the parkingId: {0}", parkingId));
                dbParking = parkingService.Get(parkingId);

                #region Solvision / PRS(KK)
                if (vendor == (int)Vendors.SOLVISION)
                {
                    logger.Debug(string.Format("Going to stop the parking at solvision."));
                    solvisionService.StopParking(dbParking.Name, parkingLot.AreaCode, dbParking.RegistrationNumber, dbParking.StopTime);
                }
                else if (vendor == (int)Vendors.PRS)
                {
                    string transactionId = CommonHelpers.GetOrderId();
                    logger.Debug(string.Format("Going to stop the parking at prs."));
                    pRSService.UpdateParking(dbParking.VendorParkingId, transactionId, parkingLot.Name, parkingLot.Name, parkingLot.CenterCoordinates, "KK", "BI", dbParking.RegistrationNumber, dbParking.StartTime, dbParking.StopTime, "0-1", false);
                }
                #endregion

                #region Cancel Push Notification
                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId15Min))
                    pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId15Min);

                if (!string.IsNullOrEmpty(dbParking.ParkingNotifications.NotificationId30Min))
                    pushNotificationService.CancelNotification(dbParking.ParkingNotifications.NotificationId30Min);

                pushNotificationService.RegisterPushNotification(dbParking.StopTime, device.RegistrationToken, "You parking has been finished", dbParking.Id);
                #endregion

                #region Sending Email
                bool IsSendEmail = Convert.ToBoolean(userSettings.FirstOrDefault(x => x.AttributeKey.Equals(UserSetting.PARKING_PDF_RECEIPT.ToString()))?.AttributeValue);
                if (IsSendEmail)
                {
                    logger.Debug(string.Format("Goint to send the parking receipt with the parkingId: {0}", parkingId));
                    logger.Debug(string.Format("Getting the user profile information with the mobileNumber: {0}", userProfile.MobileNumber));
                    UserProfiles dbUserProfile = userProfileService.Get(userProfile.MobileNumber);
                    if (userProfile.IsNull())
                        throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                    if (string.IsNullOrEmpty(userProfile.EmailAddress))
                        throw new Park4YouException(ErrorMessages.USER_EMAIL_NOT_FOUND);

                    logger.Debug(string.Format("Getting the parking report with the parkingId: {0}", parkingId));
                    ParkingReport parkingReport = parkingService.GetParkingReport(parkingId);

                    string billingHtml = _GetBillRecieptHtml();
                    string orderId = CommonHelpers.GetOrderId();
                    string userName = string.Concat(userProfile.FirstName, " ", userProfile.LastName);
                    string fileName = string.Concat("receipt-", parkingId, "-", orderId, "-", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, ".pdf");
                    string recieptId = string.Concat(parkingId, "-", orderId);
                    string html = _GetParkingReportConvertedHtml(billingHtml, parkingReport, userProfile);

                    logger.Debug(string.Format("Going to convert the parking report to pdf with the parkingId:{0}", parkingId));
                    bool IsConverted = pdfService.ConvertParkingReport(html, fileName);
                    if (IsConverted)
                    {
                        emailService.SendParkingReceiptEmail(userName, userProfile.EmailAddress, recieptId, fileName);
                    }
                }
                #endregion
            }
            logger.Debug("Auto stop parking process ended.");

            return Task.CompletedTask;
        }

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
            info.Replace("_RegistrationNumber_", parkingReport.RegistrationNumber);
            info.Replace("_AreaCode_", parkingReport.AreaCode);
            info.Replace("_StartTime_", string.Concat(parkingReport.StartTime.GetDateTimeFormat(), " ", "UTC"));
            info.Replace("_StopTime_", string.Concat(parkingReport.StopTime.GetDateTimeFormat(), " ", "UTC"));
            info.Replace("_Note_", parkingReport.ParkingNote);
            return info.ToString();
        }
    }
}