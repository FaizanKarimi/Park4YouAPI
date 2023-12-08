using System;
using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOUser
    {
        bool DeleteAccount(string mobileNumber);

        bool Update(AspNetUsers aspNetUsers);

        bool VerifyAccount(string mobileNumber, string verificationCode = null);

        bool UpdateVerificationCode(string mobileNumber, string verificationCode = null);

        bool RegisterUser(string countryCode, string mobileNumber, string country, string firstName, string lastName, string updatedBy, string userId, string deviceToken, string registrationToken, int deviceTypeId, string emailAddress, string language);

        List<VendorInformation> GetVendors();
    }
}