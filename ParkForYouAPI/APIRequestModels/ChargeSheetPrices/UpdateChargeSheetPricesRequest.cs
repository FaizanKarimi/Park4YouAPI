using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ChargeSheetPrices
{
    public class UpdateChargeSheetPricesRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string AttributeKey { get; set; }
        [Required]
        public string AttributeValue { get; set; }
    }
}