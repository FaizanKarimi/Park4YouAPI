using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IParkingRepository : IBaseRepository
    {
        Parkings GetStartParkingByMobileNumber(string mobileNumber, int parkingStatusId);

        bool Update(Parkings parking);

        Parkings Get(string mobileNumber);

        bool Delete(string mobileNumber);

        Parkings Get(int parkingId);

        Parkings GetStartParking(int parkingId, int parkingStatusId);

        Parkings Add(Parkings parking);

        Parkings GetParking(string registrationNumber);

        ParkingReport GetParkingReport(int parkingId);

        List<ParkingHistory> GetParkingHistory(string mobileNumber);

        List<ParkingAroundAreas> GetParkingAroundAreas(string latitude, string longitude);

        Parkings GetStartParkingByMobileNumber(string mobileNumber, int parkingStatusId, int cardId);

        Parkings GetStartParkingByVehicleId(string mobileNumber, int parkingStatusId, int vehicleId);

        Parkings GetStartedParking(string userId, int parkingStatusId);

        List<Parkings> GetStartedParkings(int parkingStatusId);
    }
}