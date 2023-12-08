using System.Collections.Generic;
using Infrastructure.APIResponses.Parking;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOParking
    {
        bool SaveParkingNote(int parkingId, string parkingNote);

        ActiveParkingDetailResponse GetParkingDetails(string registrationNumber);

        StartParkingResponse StartParking(int parkingId, int parkingLotId, int cardId, string mobileNumber, string registrationNumber, string parkingName, int minutes);

        StopParkingResponse StopParking(int parkingId, bool isAutoStopFlow, int seconds);

        bool SendReceipt(string mobileNumber, int parkingId);

        List<ParkingHistoryResponse> GetParkingHistory(string mobileNumber);

        List<ParkingLotModel> GetParkingAroundAreas(string latitude, string longitude);

        void CancelStartedParkingPushNotifications();
    }
}