using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IVehicleRepository : IBaseRepository
    {
        Vehicles Add(Vehicles vehicles);

        bool Delete(string mobileNumber);

        bool Update(Vehicles vehicles);

        bool Update(List<Vehicles> vehicles);

        bool Delete(Vehicles vehicles);

        List<Vehicles> GetALL(string mobileNumber);

        Vehicles Get(int vehicleId);

        Vehicles Get(string mobileNumber);
    }
}