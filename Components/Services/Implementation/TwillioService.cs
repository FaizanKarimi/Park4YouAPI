using System;
using Components.Services.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Components.Services.Implementation
{
    public class TwillioService : ITwillioService
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        #endregion

        #region Constructor
        public TwillioService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        #endregion

        #region Public Methods
        public bool SendMessage(string mobileNumber, string verificationCode)
        {
            bool IsSend = false;
            try
            {
                string userName = _appSettings.Value.TwillioUserName;
                string password = _appSettings.Value.TwillioAuthToken;
                string fromPhoneNumber = _appSettings.Value.TwillioPhoneNumber;

                TwilioClient.Init(userName, password);
                MessageResource messageResource = MessageResource.Create(to: new PhoneNumber(mobileNumber), from: new PhoneNumber(fromPhoneNumber), body: string.Concat(verificationCode, ": park4you Verification Code."));
                IsSend = true;
                TwilioClient.Invalidate();
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.ERROR_OCCURED_WHILE_SENDING_MESSAGE_USING_TWILLIO);
            }
            return IsSend;
        }
        #endregion
    }
}