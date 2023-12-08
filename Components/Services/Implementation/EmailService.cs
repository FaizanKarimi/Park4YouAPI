using System;
using System.Net;
using System.Net.Mail;
using Components.Services.Interfaces;
using Infrastructure.Cache;
using Infrastructure.DataModels;
using Infrastructure.Helpers;
using Microsoft.Extensions.Options;

namespace Components.Services.Implementation
{
    public class EmailService : IEmailService
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        #endregion

        #region Constructor
        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        #endregion

        #region Public Methods
        public void SendErrorEmail(string emailAddress, string errorMessage)
        {
            string message = "Hi,";
            message += "<br /><br />";
            message += "There is an error message occured in parkforyou api and the message is: " + errorMessage;
            this._SendErrorEmail(message, emailAddress);
        }


        public void SendParkingReceiptEmail(string username, string email, string receiptId, string fileName)
        {
            string message = "Hi " + username + ",";
            message += "<br /><br />";
            message += "This is system generated email. Please do not reply to this email.";
            message += "Thank you,<br />";
            message += "<a href=\"#\" target=\"_blank\">park4you</a>.";

            this._SendParkingEmail(username, email, message, receiptId, fileName);
        }

        public void SendProfileReportEmail(string username, UserProfiles userProfile, string fileName)
        {
            string message = "Hi " + username + ",";
            message += "<br /><br />";
            message += "This is system generated email. Please do not reply to this email.";
            message += "Thank you,<br />";
            message += "<a href=\"#\" target=\"_blank\">park4you</a>.";
            this._SendProfileReport(message, userProfile.EmailAddress, fileName);
        }
        #endregion

        #region Private Methods
        private void _SendProfileReport(string message, string userEmail, string fileName)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Host = _appSettings.Value.EmailHost;
                client.Port = Convert.ToInt32(_appSettings.Value.EmailPort);
                MailMessage mailMessage = new MailMessage(_appSettings.Value.EmailUserName, userEmail);
                mailMessage.Subject = string.Concat(_appSettings.Value.EmailProfileSubject, fileName);
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                string path = string.Concat(CommonHelpers.GetDomainName(), @"\ProfileReports\", fileName);
                Attachment attachment = new Attachment(path);
                mailMessage.Attachments.Add(attachment);
                client.Credentials = new NetworkCredential(_appSettings.Value.EmailUserName, _appSettings.Value.EmailPassword);
                client.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

        private void _SendErrorEmail(string message, string emailAddress)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Host = _appSettings.Value.EmailHost;
                client.Port = Convert.ToInt32(_appSettings.Value.EmailPort);
                MailMessage mailMessage = new MailMessage(_appSettings.Value.EmailUserName, emailAddress);
                mailMessage.Subject = string.Concat("ParkForYou api error");
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                client.Credentials = new NetworkCredential(_appSettings.Value.EmailUserName, _appSettings.Value.EmailPassword);
                client.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

        private void _SendParkingEmail(string userName, string userEmail, string message, string receiptId, string fileName)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Host = _appSettings.Value.EmailHost;
                client.Port = Convert.ToInt32(_appSettings.Value.EmailPort);
                MailMessage mailMessage = new MailMessage(_appSettings.Value.EmailUserName, userEmail);
                mailMessage.Subject = string.Concat(_appSettings.Value.EmailParkingSubject, receiptId);
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                string path = string.Concat(CommonHelpers.GetDomainName(), @"\ParkingReports\", fileName);
                Attachment attachment = new Attachment(path);
                mailMessage.Attachments.Add(attachment);
                client.Credentials = new NetworkCredential(_appSettings.Value.EmailUserName, _appSettings.Value.EmailPassword);
                client.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
        #endregion
    }
}