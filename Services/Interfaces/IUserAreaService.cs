using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IUserAreaService
    {
        bool DeleteArea(int id);

        UserAreas AddUserArea(UserAreas userArea);

        List<UserAreas> GetUserAreas(string mobileNumber);
    }
}