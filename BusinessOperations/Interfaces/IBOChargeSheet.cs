using System.Collections.Generic;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOChargeSheet
    {
        bool Update(ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices);

        ChargeSheets Get(int id);
    }
}