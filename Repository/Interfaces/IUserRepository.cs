using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        bool Update(AspNetUsers aspNetUsers);

        bool Delete(AspNetUsers aspNetUsers);

        AspNetUsers Get(string mobileNumber);

        AspNetUsers GetUser(string id);

        List<VendorInformation> GetVendors();
    }
}