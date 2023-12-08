using System;

namespace Infrastructure.DataModels
{
    public class Vehicles
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string RegistrationId { get; set; }
        public string RegistrationNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public bool IsLatest { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}