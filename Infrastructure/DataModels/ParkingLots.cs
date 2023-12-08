using System;

namespace Infrastructure.DataModels
{
    public partial class ParkingLots
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public string CenterCoordinates { get; set; }
        public string GeoCoordinates { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}