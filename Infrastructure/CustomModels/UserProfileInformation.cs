using System.Collections.Generic;

namespace Infrastructure.CustomModels
{
    public class UserProfileInformation
    {
        public string MobileNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetNumber { get; set; }
        public string ZipCode { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string EmailAddress { get; set; }
        public List<UserCardsInformation> UserCards { get; set; }
        public List<UserVehicles> Vehicles { get; set; }
        public List<UserAreaModel> Areas { get; set; }
        public List<UserSettingInformation> UserSettings { get; set; }
        public UserStartedParking Parking { get; set; }
    }

    public class UserCardsInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CardVerificationValue { get; set; }
        public string CardExpiry { get; set; }
        public string PaymentType { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UserVehicles
    {
        public int Id { get; set; }
        public string RegistrationId { get; set; }
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }
        public bool IsLatest { get; set; }
    }

    public class UserAreaModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string AreaCode { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public bool IsLatest { get; set; }
    }

    public class UserStartedParking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CenterPoint { get; set; }
        public List<LongitudeLatitudeModel> AreaGeoCoordinates { get; set; }
        public string AreaCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string TotalPrice { get; set; }
        public bool? IsFixed { get; set; }
        public int ParkingLotId { get; set; }
        public string ParkingNote { get; set; }
        public string VehicleName { get; set; }
        public int RemainingSeconds { get; set; }
        public int TotatSeconds { get; set; }
        public string BasePrice { get; set; }
    }

    public class LongitudeLatitudeModel
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class UserSettingInformation
    {
        public int Id { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
    }
}