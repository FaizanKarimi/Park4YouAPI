using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IUserSettingService
    {
        List<UserSettings> Get(string mobileNumber);

        bool Update(UserSettings userSettings);
    }
}