using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IUserProfileRepository : IBaseRepository
    {
        UserProfiles Get(string mobileNumber);

        UserProfiles GetByUserId(string userId);

        bool Update(UserProfiles userProfiles);

        bool Delete(UserProfiles userProfile);

        bool Add(UserProfiles userProfile);
    }
}