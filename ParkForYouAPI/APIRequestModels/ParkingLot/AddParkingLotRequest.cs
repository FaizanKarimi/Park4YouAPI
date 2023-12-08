using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ParkForYouAPI.APIRequestModels.ChargeSheet;
using ParkForYouAPI.APIRequestModels.ChargeSheetPrices;

namespace ParkForYouAPI.APIRequestModels.ParkingLot
{
    public class AddParkingLotRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string AreaCode { get; set; }
        [Required]
        public string CenterCoordinates { get; set; }
        [Required]
        public string GeoCoordinates { get; set; }

        public SaveChargeSheetRequest saveChargeSheetRequest { get; set; }
        public List<SaveChargeSheetPricesRequest> saveChargeSheetPricesRequests { get; set; }
    }
}