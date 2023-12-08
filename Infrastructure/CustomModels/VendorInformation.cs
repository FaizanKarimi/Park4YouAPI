using System;

namespace Infrastructure.CustomModels
{
    public class VendorInformation
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool LockoutEnabled { get; set; }
        public string Name { get; set; }
    }
}