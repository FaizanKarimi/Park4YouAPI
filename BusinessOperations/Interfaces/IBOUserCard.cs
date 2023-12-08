using System.Collections.Generic;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOUserCard
    {
        bool Delete(string mobileNumber, int cardId);

        List<UserCards> GetALL(string mobileNumber);

        UserCards Add(UserCards userCards);

        bool MarkedUserCardDefault(int cardId, string mobileNumber);
    }
}