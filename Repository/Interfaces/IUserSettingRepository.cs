using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IUserSettingRepository : IBaseRepository
    {
        List<UserSettings> Get(string mobileNumber);

        bool Update(UserSettings userSettings);

        UserSettings Get(int Id);

        bool Add(List<UserSettings> userSettings);

        bool Delete(string mobileNumber);
    }
}