using System.Collections.Generic;
using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class UserSettingRepository : IUserSettingRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public bool Add(List<UserSettings> userSettings)
        {
            const string sqlQuery = @"INSERT INTO UserSettings (UserId, MobileNumber, AttributeKey, AttributeValue, CreatedOn, UpdatedOn)
                                      VALUES (@UserId, @MobileNumber, @AttributeKey, @AttributeValue, @CreatedOn, @UpdatedOn)";
            int result = UnitOfWork.Connection.Execute(sqlQuery, userSettings, UnitOfWork.Transaction);
            return result > 0;
        }

        public bool Delete(string mobileNumber)
        {
            const string sqlQuery = @"DELETE FROM UserSettings WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new { MobileNumber = mobileNumber }, UnitOfWork.Transaction);
            return result > 0;
        }

        public List<UserSettings> Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM UserSettings WHERE MobileNumber = @MobileNumber";
            return UnitOfWork.Connection.Query<UserSettings>(sqlQuery, new { MobileNumber = mobileNumber }).ToList();
        }

        public UserSettings Get(int Id)
        {
            const string sqlQuery = @"SELECT * FROM UserSettings WHERE Id = @Id";
            return UnitOfWork.Connection.QueryFirstOrDefault<UserSettings>(sqlQuery, new { Id = Id });
        }

        public bool Update(UserSettings userSettings)
        {
            const string sqlQuery = @"UPDATE UserSettings SET AttributeValue = @AttributeValue, UpdatedOn = @UpdatedOn  WHERE Id = @Id AND AttributeKey = @AttributeKey";
            int result = UnitOfWork.Connection.Execute(sqlQuery, userSettings, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}