using System.Collections.Generic;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOUserSetting
    {
        bool Update(UserSettings userSettings);
    }
}