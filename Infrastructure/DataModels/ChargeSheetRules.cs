using System;

namespace Infrastructure.DataModels
{
    public partial class ChargeSheetRules
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}