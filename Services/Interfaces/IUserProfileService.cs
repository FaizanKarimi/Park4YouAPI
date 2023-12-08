using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IUserProfileService
    {
        UserProfiles Get(string mobileNumber);

        bool UpdateVerficationCode(string mobileNumber, string verificationCode);

        bool AddUserProfile(UserProfiles userProfile, Devices device, string language);

        bool Update(UserProfiles userProfile);

        bool UpdateByUserId(UserProfiles userProfile);

        UserProfileInformation GetUserProfileInformation(string mobileNumber);

        bool AddUserProfile(UserProfiles userProfile);

        UserProfiles GetByUserId(string userId);
    }
}