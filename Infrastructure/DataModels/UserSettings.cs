using System;

namespace Infrastructure.DataModels
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}