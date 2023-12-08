using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IChargeSheetPriceRepository : IBaseRepository
    {
        List<ChargeSheetPrices> Get(int chargeSheetId);

        bool Update(List<ChargeSheetPrices> chargeSheetPrices);

        bool Add(List<ChargeSheetPrices> chargeSheetPrices);
    }
}