using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IUserAreaRepository : IBaseRepository
    {
        bool Delete(int id);

        UserAreas Get(int id);

        bool Update(UserAreas userArea);

        bool Update(List<UserAreas> userArea);

        List<UserAreas> GetALL(string mobileNumber);

        UserAreas Add(UserAreas userAreas);
    }
}