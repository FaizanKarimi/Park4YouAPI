using System.Runtime.CompilerServices;
using Components.Services.Interfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using NLog;
using NLog.Web;

namespace Components.Services.Implementation
{
    public class Logging : ILogging
    {
        #region Private Members
        private Logger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public Logging(IHttpContextAccessor httpContextAccessor)
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public Methods
        public void Debug(string message = "", string emailAddress = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        {
            string Message = _GetMessage(message, emailAddress, memberName, lineNumber);
            _logger.Debug(Message);
        }
        #endregion

        #region Private Methods
        private string _GetMessage(string message, string emailAddress, string methodName, int lineNumber)
        {
            emailAddress = _GetEmailAddress(emailAddress);
            return string.Format("Message=[{0}], EmailAddress=[{1}], Method=[{2}], LineNumber=[{3}]", message, emailAddress, methodName, lineNumber);
        }

        private string _GetEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                emailAddress = _httpContextAccessor.GetCurrentUserEmail();

            return emailAddress;
        }
        #endregion
    }
}