using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IUserService
    {
        bool DeleteAccount(string mobileNumber);

        bool Update(AspNetUsers aspNetUsers);

        bool VerifyAccount(string mobileNumber, string verificationCode);

        List<VendorInformation> GetVendors();
    }
}