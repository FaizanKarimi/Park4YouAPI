using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ParkForYouAPI.APIRequestModels.ChargeSheetPrices;

namespace ParkForYouAPI.APIRequestModels.ChargeSheet
{
    public class UpdateChargeSheetRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<UpdateChargeSheetPricesRequest> updateChargeSheetPricesRequest { get; set; }
    }
}