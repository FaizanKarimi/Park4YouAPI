using System;

namespace Infrastructure.CustomModels
{
    public class ParkingHistory
    {
        public int ParkingId { get; set; }
        public decimal Price { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public string VehicleName { get; set; }
        public string AreaCode { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public int? RemainingMinutes { get; set; }
    }
}