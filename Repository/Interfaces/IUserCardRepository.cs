using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IUserCardRepository : IBaseRepository
    {
        UserCards Get(int id);

        UserCards Add(UserCards userCards);

        List<UserCards> GetALL(string mobileNumber);

        bool Update(UserCards userCards);

        bool Update(List<UserCards> userCards);

        bool Delete(UserCards userCards);
    }
}