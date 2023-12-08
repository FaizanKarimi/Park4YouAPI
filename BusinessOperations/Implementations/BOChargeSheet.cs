using System.Collections.Generic;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Infrastructure.DataModels;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOChargeSheet : IBOChargeSheet
    {
        #region Private Members
        private readonly IChargeSheetService _chargeSheetService;
        private readonly ILogging _logging;
        #endregion

        #region Constructor
        public BOChargeSheet(IChargeSheetService chargeSheetService, ILogging logging)
        {
            _chargeSheetService = chargeSheetService;
            _logging = logging;
        }

        public ChargeSheets Get(int id)
        {
            return _chargeSheetService.Get(id);
        }
        #endregion

        public bool Update(ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices)
        {
            return _chargeSheetService.Update(chargeSheet, chargeSheetPrices);
        }
    }
}