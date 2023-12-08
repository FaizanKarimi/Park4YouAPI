using System;

namespace Infrastructure.DataModels
{
    public partial class ChargeSheets
    {
        public int Id { get; set; }
        public int ParkingLotId { get; set; }
        public string BaseCurrency { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}