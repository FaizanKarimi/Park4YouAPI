using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public bool Delete(AspNetUsers aspNetUsers)
        {
            const string sqlQuery = @"UPDATE AspNetUsers SET UserName = @UserName, Email = @Email, PhoneNumber = @PhoneNumber, NormalizedUserName = @NormalizedUserName WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                UserName = aspNetUsers.UserName,
                Email = aspNetUsers.Email,
                PhoneNumber = aspNetUsers.PhoneNumber,
                NormalizedUserName = aspNetUsers.NormalizedEmail,
                MobileNumber = aspNetUsers.MobileNumber
            }, UnitOfWork.Transaction);
            return result > 0;
        }

        public AspNetUsers Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM AspNetUsers WHERE MobileNumber = @MobileNumber";
            return UnitOfWork.Connection.QueryFirstOrDefault<AspNetUsers>(sqlQuery, new { MobileNumber = mobileNumber });
        }

        public AspNetUsers GetUser(string id)
        {
            const string sqlQuery = @"SELECT * FROM AspNetUsers WHERE Id = @Id";
            return UnitOfWork.Connection.QueryFirstOrDefault<AspNetUsers>(sqlQuery, new { Id = id });
        }

        public List<VendorInformation> GetVendors()
        {
            const string storedProcedureName = "GetVendors";
            return UnitOfWork.Connection.Query<VendorInformation>(storedProcedureName, commandType: CommandType.StoredProcedure).ToList();
        }

        public bool Update(AspNetUsers aspNetUsers)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE AspNetUsers SET PhoneNumberConfirmed = @PhoneNumberConfirmed WHERE MobileNumber = @MobileNumber";
            UnitOfWork.Connection.Execute(sqlQuery, new { PhoneNumberConfirmed = aspNetUsers.PhoneNumberConfirmed, MobileNumber = aspNetUsers.MobileNumber }, UnitOfWork.Transaction);
            return IsUpdated;
        }
    }
}