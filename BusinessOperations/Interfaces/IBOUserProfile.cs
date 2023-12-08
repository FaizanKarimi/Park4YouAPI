using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOUserProfile
    {
        bool Update(UserProfiles userProfiles);

        UserProfileInformation GetUserProfileInformation(string mobileNumber);

        UserProfiles Get(string userId);

        bool UpdateByUserId(UserProfiles userProfile);

        bool SendDataToEmail(string mobileNumber);

        bool AddUserProfile(UserProfiles userProfile);
    }
}