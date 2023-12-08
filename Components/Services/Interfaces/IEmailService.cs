using Infrastructure.DataModels;

namespace Components.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Send parking report to user email address.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="receiptId"></param>
        /// <param name="fileName"></param>
        void SendParkingReceiptEmail(string username, string email, string receiptId, string fileName);

        /// <summary>
        /// Send profile report to user email address.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userProfile"></param>
        /// <param name="fileName"></param>
        void SendProfileReportEmail(string username, UserProfiles userProfile, string fileName);

        /// <summary>
        /// Send the error email.
        /// </summary>
        /// <param name="emailAddress"></param>
        void SendErrorEmail(string emailAddress, string message);
    }
}