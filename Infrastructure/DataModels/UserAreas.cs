using System;

namespace Infrastructure.DataModels
{
    public class UserAreas
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string Country { get; set; }
        public string AreaCode { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public bool IsLatest { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}