using System;

namespace Infrastructure.DataModels
{
    public class UserCards
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CardVerficationValue { get; set; }
        public string PaymentType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public string CardExpiry { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}