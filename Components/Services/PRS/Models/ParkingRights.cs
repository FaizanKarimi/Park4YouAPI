using System;

namespace Components.Services.PRS.Models
{
    public class ParkingRights
    {
        public string prid { get; set; }
        public string validityBegin { get; set; }
        public string validityEnd { get; set; }
        public string providerId { get; set; }
        public string transactionId { get; set; }
        public string vehicleId { get; set; }
        public string areaId { get; set; }
        public DateTime lastModified { get; set; }
        public string sellingPointLocation { get; set; }
    }
}