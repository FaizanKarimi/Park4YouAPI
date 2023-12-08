using System;

namespace Infrastructure.CustomModels
{
    public class ParkingLotInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public string CenterCoordinates { get; set; }
        public string GeoCoordinates { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
        public int ChargeSheetId { get; set; }
    }
}