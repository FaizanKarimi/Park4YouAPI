using System;

namespace Components.Services.PRS.Models
{
    public class ParkingRight
    {
        public string prid { get; set; }
        public string providerId { get; set; }
        public string transactionId { get; set; }
        public string productDescription { get; set; }
        public string sellingPointId { get; set; }
        public string sellingPointLocation { get; set; }
        public string areaManagerId { get; set; }
        public string areaId { get; set; }
        public string vehicleId { get; set; }
        public string normalizedVehicleId { get; set; }
        public string validityBegin { get; set; }
        public string validityEnd { get; set; }
        public string validityHours { get; set; }
        public bool validityCancelled { get; set; }
        public DateTime lastModified { get; set; }
    }
}