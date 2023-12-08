using System;

namespace Infrastructure.DataModels
{
    public class Parkings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string VendorParkingId { get; set; }
        public string MobileNumber { get; set; }
        public int CardId { get; set; }
        public int VehicleId { get; set; }
        public string Name { get; set; }
        public int ParkingLotId { get; set; }
        public string AreaCode { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public decimal Price { get; set; }
        public bool? IsFixed { get; set; }
        public bool IsLatest { get; set; }
        public int ParkingStatusId { get; set; }
        public string ParkingStatus { get; set; }
        public string ParkingNote { get; set; }
        public int? TotalPrice { get; set; }
        public bool IsDeleted { get; set; }

        public ParkingNotifications ParkingNotifications { get; set; }
    }
}