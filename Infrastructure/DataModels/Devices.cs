using System;

namespace Infrastructure.DataModels
{
    public class Devices
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string DeviceToken { get; set; }
        public string RegistrationToken { get; set; }
        public int? DeviceTypeId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}