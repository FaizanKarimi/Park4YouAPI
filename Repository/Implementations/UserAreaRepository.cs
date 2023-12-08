using System.Collections.Generic;
using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class UserAreaRepository : IUserAreaRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public UserAreas Add(UserAreas userAreas)
        {
            const string sqlQuery = @"INSERT INTO UserAreas (UserId, MobileNumber, Country, AreaCode, City, Town, IsLatest, IsDeleted, CreatedOn, UpdatedOn) 
                                        VALUES (@UserId, @MobileNumber, @Country, @AreaCode, @City, @Town, @IsLatest, @IsDeleted, @CreatedOn, @UpdatedOn)
                                        SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int primaryId = UnitOfWork.Connection.Query<int>(sqlQuery, new
            {
                UserId = userAreas.UserId,
                MobileNumber = userAreas.MobileNumber,
                Country = userAreas.Country,
                AreaCode = userAreas.AreaCode,
                City = userAreas.City,
                Town = userAreas.Town,
                IsLatest = userAreas.IsLatest,
                IsDeleted = userAreas.IsDeleted,
                CreatedOn = userAreas.CreatedOn,
                UpdatedOn = userAreas.UpdatedOn
            }, UnitOfWork.Transaction).Single();
            userAreas.Id = primaryId;
            return userAreas;
        }

        public bool Delete(int id)
        {
            bool IsDeleted = false;
            const string sqlQuery = @"UPDATE UserAreas SET IsDeleted = @IsDeleted WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new { IsDeleted = true, Id = id }, UnitOfWork.Transaction);
            IsDeleted = result > 0;
            return IsDeleted;
        }

        public UserAreas Get(int id)
        {
            const string sqlQuery = @"SELECT * FROM UserAreas WHERE Id = @Id AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<UserAreas>(sqlQuery, new { Id = id, IsDeleted = false });
        }

        public List<UserAreas> GetALL(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM UserAreas WHERE MobileNumber = @MobileNumber AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.Query<UserAreas>(sqlQuery, new { MobileNumber = mobileNumber, IsDeleted = false }, UnitOfWork.Transaction).ToList();
        }

        public bool Update(UserAreas userArea)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE UserAreas SET IsLatest = @IsLatest WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsLatest = userArea.IsLatest,
                Id = userArea.Id
            }, UnitOfWork.Transaction);
            IsUpdated = result > 0;
            return IsUpdated;
        }

        public bool Update(List<UserAreas> userArea)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE UserAreas SET IsLatest = @IsLatest WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, userArea, UnitOfWork.Transaction);
            IsUpdated = result > 0;
            return IsUpdated;
        }
    }
}