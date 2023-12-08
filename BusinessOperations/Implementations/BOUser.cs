using System;
using System.Collections.Generic;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Helpers;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOUser : IBOUser
    {
        #region Private Members
        private readonly IUserService _userService;
        private readonly ITwillioService _twillioService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public BOUser(IUserService userService, ITwillioService twillioService, IUserProfileService userProfileService, ILogging logging)
        {
            _userService = userService;
            _twillioService = twillioService;
            _userProfileService = userProfileService;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public bool DeleteAccount(string mobileNumber)
        {
            bool IsDeleted = false;
            _logger.Debug("Account deletion process started.");
            IsDeleted = _userService.DeleteAccount(mobileNumber);
            _logger.Debug("Account deletion process ended.");
            return IsDeleted;
        }

        public List<VendorInformation> GetVendors()
        {
            _logger.Debug("Getting all the vendors.");
            return _userService.GetVendors();
        }

        public bool RegisterUser(string countryCode, string mobileNumber, string country, string firstName, string lastName, string updatedBy, string userId, string deviceToken, string registrationToken, int deviceTypeId, string emailAddress, string language)
        {
            bool IsRegistered = false;
            _logger.Debug("Generating the randon number.");
            string code = CommonHelpers.GenrateRandomNumber();
            _logger.Debug(string.Format("Sending the verification code to registered mobile number: {0}", code));
            bool IsMessageSend = _twillioService.SendMessage(mobileNumber, code);
            if (IsMessageSend)
            {
                _logger.Debug("Message is send to registered mobile number");
                UserProfiles userProfile = new UserProfiles()
                {
                    UserId = userId,
                    EmailAddress = emailAddress,
                    MobileNumber = mobileNumber,
                    Country = country,
                    CountryCode = countryCode,
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = updatedBy,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    VerificationCode = code
                };
                Devices device = new Devices()
                {
                    MobileNumber = mobileNumber,
                    UserId = userId,
                    DeviceToken = deviceToken,
                    RegistrationToken = registrationToken,
                    DeviceTypeId = deviceTypeId
                };
                _logger.Debug("Adding the user profile information.");
                IsRegistered = _userProfileService.AddUserProfile(userProfile, device, language);
            }
            return IsRegistered;
        }

        public bool Update(AspNetUsers aspNetUsers)
        {
            _logger.Debug("Updating the user.");
            return _userService.Update(aspNetUsers);
        }

        public bool UpdateVerificationCode(string mobileNumber, string verificationCode)
        {
            if (string.IsNullOrEmpty(verificationCode))
            {
                _logger.Debug("Generating the verificationCode.");
                verificationCode = CommonHelpers.GenrateRandomNumber();
            }

            _logger.Debug(string.Format("Sending the verification code message: {0}", verificationCode));
            _twillioService.SendMessage(mobileNumber, verificationCode);

            return _userProfileService.UpdateVerficationCode(mobileNumber, verificationCode);
        }

        public bool VerifyAccount(string mobileNumber, string verificationCode)
        {
            bool IsVerified = false;
            _logger.Debug("Verifying the account.");
            IsVerified = _userService.VerifyAccount(mobileNumber, verificationCode);
            return IsVerified;
        }
        #endregion
    }
}