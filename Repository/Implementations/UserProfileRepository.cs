using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public UserProfiles GetByUserId(string userId)
        {
            const string sqlQuery = @"SELECT * FROM UserProfiles WHERE UserId = @UserId";
            return UnitOfWork.Connection.QueryFirstOrDefault<UserProfiles>(sqlQuery, new { UserId = userId });
        }

        public UserProfiles Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM UserProfiles WHERE MobileNumber = @MobileNumber AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<UserProfiles>(sqlQuery, new { MobileNumber = mobileNumber, IsDeleted = false });
        }

        public bool Update(UserProfiles userProfiles)
        {
            const string sqlQuery = @"UPDATE UserProfiles SET 
                                        VerificationCode = @VerificationCode, 
                                        EmailAddress = @EmailAddress,
                                        FirstName = @FirstName,
                                        LastName = @LastName,
                                        StreetNumber = @StreetNumber,
                                        ZipCode = @ZipCode,
                                        Town = @Town,
                                        Country = @Country,
                                        UpdatedBy = @UpdatedBy,
                                        UpdatedOn = @UpdatedOn,
                                        MobileNumber = @MobileNumber
                                     WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                VerificationCode = userProfiles.VerificationCode,
                EmailAddress = userProfiles.EmailAddress,
                FirstName = userProfiles.FirstName,
                LastName = userProfiles.LastName,
                StreetNumber = userProfiles.StreetNumber,
                ZipCode = userProfiles.ZipCode,
                Town = userProfiles.Town,
                Country = userProfiles.Country,
                UpdatedBy = userProfiles.UpdatedBy,
                UpdatedOn = userProfiles.UpdatedOn,
                MobileNumber = userProfiles.MobileNumber,
                Id = userProfiles.Id
            }, UnitOfWork.Transaction);
            return result > 0;
        }

        public bool Add(UserProfiles userProfile)
        {
            bool IsAdded = false;
            const string sqlQuery = @"INSERT INTO UserProfiles (UserId, EmailAddress, MobileNumber, FirstName, LastName, StreetNumber, ZipCode, Town, Country, CountryCode, IsDeleted, VerificationCode, UpdatedBy, CreatedOn, UpdatedOn)
                                    VALUES (@UserId, @EmailAddress, @MobileNumber, @FirstName, @LastName, @StreetNumber, @ZipCode, @Town, @Country, @CountryCode, @IsDeleted, @VerificationCode, @UpdatedBy, @CreatedOn, @UpdatedOn)";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                UserId = userProfile.UserId,
                EmailAddress = userProfile.EmailAddress,
                MobileNumber = userProfile.MobileNumber,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                StreetNumber = userProfile.StreetNumber,
                ZipCode = userProfile.ZipCode,
                Town = userProfile.Town,
                Country = userProfile.Country,
                CountryCode = userProfile.CountryCode,
                IsDeleted = userProfile.IsDeleted,
                VerificationCode = userProfile.VerificationCode,
                UpdatedBy = userProfile.UpdatedBy,
                CreatedOn = userProfile.CreatedOn,
                UpdatedOn = userProfile.UpdatedOn
            }, UnitOfWork.Transaction);
            IsAdded = result > 0;
            return IsAdded;
        }

        public bool Delete(UserProfiles userProfile)
        {
            const string sqlQuery = @"UPDATE UserProfiles SET IsDeleted = @IsDeleted WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsDeleted = true,
                Id = userProfile.Id
            }, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}