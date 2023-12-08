using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public partial class ParkingLots
    {
        public ParkingLots()
        {
            this.ChargeSheetPrices = new List<ChargeSheetPrices>();
        }

        public ChargeSheets ChargeSheet { get; set; }
        public List<ChargeSheetPrices> ChargeSheetPrices { get; set; }
    }
}
