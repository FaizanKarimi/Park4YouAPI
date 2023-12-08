using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public partial class ChargeSheets
    {
        public ChargeSheets()
        {
            this.ChargeSheetPrices = new List<ChargeSheetPrices>();
        }

        public string OwnerName { get; set; }
        public List<ChargeSheetPrices> ChargeSheetPrices { get; set; }
    }
}