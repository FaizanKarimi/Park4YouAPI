using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IUserCardService
    {
        bool Delete(UserCards userCards);

        List<UserCards> GetALL(string mobileNumber);

        UserCards Get(int CardId);

        UserCards Add(UserCards userCards);

        bool SetDefaultUserCard(int cardId, string mobileNumber);
    }
}