using System.Collections.Generic;
using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class UserCardRepository : IUserCardRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public UserCards Add(UserCards userCards)
        {
            const string sqlQuery = @"INSERT INTO UserCards (UserId, MobileNumber, Name, CardNumber, CardVerficationValue, PaymentType, IsDefault, IsDeleted, UpdatedBy, CardExpiry, CreatedOn, UpdatedOn) 
                                      VALUES (@UserId, @MobileNumber, @Name, @CardNumber, @CardVerficationValue, @PaymentType, @IsDefault, @IsDeleted, @UpdatedBy, @CardExpiry, @CreatedOn, @UpdatedOn)
                                      SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, userCards, UnitOfWork.Transaction).Single();
            userCards.Id = result;
            return userCards;
        }

        public bool Delete(UserCards userCards)
        {
            bool IsDeleted = false;
            const string sqlQuery = @"UPDATE UserCards SET IsDeleted = @IsDeleted, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsDeleted = true,
                UpdatedBy = userCards.UpdatedBy,
                UpdatedOn = userCards.UpdatedOn,
                Id = userCards.Id
            }, UnitOfWork.Transaction);
            IsDeleted = result > 0;
            return IsDeleted;
        }

        public UserCards Get(int id)
        {
            const string sqlQuery = @"SELECT * FROM UserCards WHERE Id = @Id AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<UserCards>(sqlQuery, new { Id = id, IsDeleted = false });
        }

        public List<UserCards> GetALL(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM UserCards WHERE MobileNumber = @MobileNumber AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.Query<UserCards>(sqlQuery, new { MobileNumber = mobileNumber, IsDeleted = false }).ToList();
        }

        public bool Update(UserCards userCards)
        {
            const string sqlQuery = @"UPDATE UserCards SET IsDeleted = @IsDeleted, IsDefault = @IsDefault, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsDeleted = false,
                IsDefault = userCards.IsDefault,
                UpdatedOn = userCards.UpdatedOn,
                UpdatedBy = userCards.UpdatedBy,
                Id = userCards.Id
            }, UnitOfWork.Transaction);
            return result > 0;
        }

        public bool Update(List<UserCards> userCards)
        {
            const string sqlQuery = @"UPDATE UserCards SET IsDeleted = @IsDeleted, IsDefault = @IsDefault, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, userCards, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}