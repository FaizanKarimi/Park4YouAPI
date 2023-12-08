using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IParkingService
    {
        Parkings GetStartedParkingByMobileNumber(string mobileNumber);

        bool SaveParkingNote(int parkingId, string parkingNote);

        Parkings Get(int parkingId);

        Parkings GetParkingDetails(string registrationNumber);

        Parkings StartParking(Parkings parking, ParkingNotifications parkingNotifications);

        bool StopParking(Parkings parking, PaymentOrders paymentOrder);

        ParkingReport GetParkingReport(int parkingId);

        List<ParkingHistory> GetParkingHistory(string mobileNumber);

        List<ParkingAroundAreas> GetParkingAroundAreas(string latitude, string longitude);

        Parkings Update(Parkings parking, ParkingNotifications parkingNotifications);

        Parkings GetStartParking(int parkingId);

        Parkings GetStartedParking(string userId);

        string GetParkingPushNotificationMessage(Parkings parkings);

        List<Parkings> GetStartedParkings();
    }
}