using System;

namespace Infrastructure.DataModels
{
    public partial class ChargeSheetPrices
    {
        public int Id { get; set; }
        public int ChargeSheetId { get; set; }
        public int ChargeSheetRuleId { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}