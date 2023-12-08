using System;

namespace Infrastructure.DataModels
{
    public partial class ChargeSheetPladIds
    {
        public int Id { get; set; }
        public int ChargeSheetId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}