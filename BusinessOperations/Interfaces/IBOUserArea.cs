using System.Collections.Generic;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOUserArea
    {
        bool Delete(int id);

        UserAreas Add(UserAreas userArea);

        List<UserAreas> GetUserAreas(string mobileNumber);
    }
}