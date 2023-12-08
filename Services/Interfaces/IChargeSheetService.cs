using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IChargeSheetService
    {
        ChargeSheets Get(int id);

        bool Update(ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices);
    }
}