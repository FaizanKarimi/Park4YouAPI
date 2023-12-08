using System;

namespace Infrastructure.DataModels
{
    public class CardValidation
    {
        public long CardId { get; set; }
        public string MobileNumber { get; set; }
        public long Id { get; set; }
        public long MerchantId { get; set; }
        public bool Accepted { get; set; }
        public string MetaDataType { get; set; }
        public string Origin { get; set; }
        public string Brand { get; set; }
        public string Bin { get; set; }
        public string Last4 { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Country { get; set; }
        public bool? Is3dSecure { get; set; }
        public string IssuedTo { get; set; }
        public string Hash { get; set; }
        public string Number { get; set; }
        public string CustomerIP { get; set; }
        public string CustomerCountry { get; set; }
        public bool? FraudSuspected { get; set; }
        public string NinCountryCode { get; set; }
        public string NinGender { get; set; }
        public string URL { get; set; }
        public int? AgreementId { get; set; }
        public string Language { get; set; }
        public string ContinueURL { get; set; }
        public string CancelURL { get; set; }
        public string CallbackURL { get; set; }
        public string PaymentMethods { get; set; }
        public string LinkBrandingId { get; set; }
        public string Version { get; set; }
        public int? OperationId { get; set; }
        public string OperationType { get; set; }
        public bool? Pending { get; set; }
        public string QPStatusCode { get; set; }
        public string QPStatusMessage { get; set; }
        public DateTime? OperationCreatedDate { get; set; }
        public string TestMode { get; set; }
        public string Acquirer { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}