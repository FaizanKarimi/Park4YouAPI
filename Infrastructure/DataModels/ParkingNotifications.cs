using System;

namespace Infrastructure.DataModels
{
    public class ParkingNotifications
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ParkingId { get; set; }
        public string NotificationId15Min { get; set; }
        public string NotificationId30Min { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}