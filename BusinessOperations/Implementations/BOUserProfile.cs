using System;
using System.IO;
using System.Text;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOUserProfile : IBOUserProfile
    {
        #region Private Members
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public BOUserProfile(IUserProfileService userProfileService, IEmailService emailService, IPdfService pdfService, ILogging logging)
        {
            _userProfileService = userProfileService;
            _emailService = emailService;
            _pdfService = pdfService;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public UserProfileInformation GetUserProfileInformation(string mobileNumber)
        {
            return _userProfileService.GetUserProfileInformation(mobileNumber);
        }

        public UserProfiles Get(string userId)
        {
            return _userProfileService.GetByUserId(userId);
        }

        public bool SendDataToEmail(string mobileNumber)
        {
            bool IsSend = false;
            _logger.Debug(string.Format("Getting the user profile information with the mobile number: {0}", mobileNumber));
            UserProfiles userProfile = _userProfileService.Get(mobileNumber);

            #region Validations
            if (userProfile.IsNull())
                throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

            if (string.IsNullOrEmpty(userProfile.EmailAddress))
                throw new Park4YouException(ErrorMessages.USER_EMAIL_NOT_FOUND);
            #endregion

            string profileHtml = _GetProfileHtml();
            string html = _GetProfileReportConvertedHtml(profileHtml, userProfile);
            string fileName = string.Concat("profile-", mobileNumber, "-", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, ".pdf");
            string userName = string.Concat(userProfile.FirstName, " ", userProfile.LastName);

            _logger.Debug(string.Format("Going to convert the profile report into pdf format with the filename: {0}", fileName));
            bool IsConverted = _pdfService.ConvertProfileReport(html, fileName);
            _logger.Debug(string.Format("PDF is converted: {0}", IsConverted));
            if (IsConverted)
            {
                _logger.Debug(string.Format("Going to send the profile report to user email address: {0}", userProfile.EmailAddress));
                _emailService.SendProfileReportEmail(userName, userProfile, fileName);
            }
            return IsSend;
        }

        public bool Update(UserProfiles userProfile)
        {
            return _userProfileService.Update(userProfile);
        }

        public bool UpdateByUserId(UserProfiles userProfile)
        {
            return _userProfileService.UpdateByUserId(userProfile);
        }

        public bool AddUserProfile(UserProfiles userProfile)
        {
            return _userProfileService.AddUserProfile(userProfile);
        }
        #endregion

        #region Private Methods
        private string _GetProfileReportConvertedHtml(string html, UserProfiles userProfile)
        {
            StringBuilder info = new StringBuilder(html);
            info.Replace("_USERNAME_", userProfile.EmailAddress);
            info.Replace("_MOBILENUMBER_", userProfile.MobileNumber);
            info.Replace("_FIRSTNAME_", userProfile.FirstName);
            info.Replace("_LASTNAME_", userProfile.LastName);
            info.Replace("_STREETNUMBER_", userProfile.StreetNumber);
            info.Replace("_ZIPCODE_", userProfile.ZipCode);
            info.Replace("_TOWN_", userProfile.Town);
            info.Replace("_COUNTRYCODE_", userProfile.CountryCode);
            info.Replace("_COUNTRY_", userProfile.Country);
            return info.ToString();
        }

        private string _GetProfileHtml()
        {
            var baseDirectory = CommonHelpers.GetDomainName();
            var filePath = string.Concat(baseDirectory, @"\HtmlTemplates\ProfileReport.html");
            string text = File.ReadAllText(filePath);
            return text;
        }
        #endregion
    }
}