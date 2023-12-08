namespace Infrastructure.Cache
{
    public class AppSettings
    {
        #region Twilio
        public string TwillioUserName { get; set; }
        public string TwillioAuthToken { get; set; }
        public string TwillioPhoneNumber { get; set; }
        #endregion

        #region Jwt (Json Web Token)
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireTime { get; set; }
        #endregion

        #region QuickPay
        public string QuickPayBaseUrl { get; set; }
        public string QuickPayPaymentUrl { get; set; }
        public string QuickPayAcceptVersion { get; set; }
        public string QuickPayPassword { get; set; }
        #endregion

        #region Email
        public string EmailHost { get; set; }
        public string EmailPort { get; set; }
        public string EmailParkingSubject { get; set; }
        public string EmailProfileSubject { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        #endregion

        #region Solvision
        public string SolvisionAPIUserName { get; set; }
        public string SolvisionAPIPassword { get; set; }
        public string SolvisionBaseAddress { get; set; }
        public string SolvisionStartParkingUrl { get; set; }
        public string SolvisionStopParkingUrl { get; set; }
        public string SolvisionLoginUrl { get; set; }
        #endregion

        #region Notification
        public string NotificationUrl { get; set; }
        public string NotificationAPIKey { get; set; }
        public string NotificationAppId { get; set; }
        public string NotificationAuthKey { get; set; }
        #endregion

        #region Parking VendorId's
        public string ParkingVendorIds { get; set; }
        #endregion

        #region PRS
        public string PRSParkingUrl { get; set; }
        public string PRSProviderId { get; set; }
        public string PRSToken { get; set; }
        #endregion

        #region PDF
        public string PdfUrl { get; set; }
        #endregion

        #region QUARTZ
        public string QuartzDataSource { get; set; }
        public string QuartzConnectionString { get; set; }
        public string QuartzIdleTimeWait { get; set; }
        #endregion
    }
}