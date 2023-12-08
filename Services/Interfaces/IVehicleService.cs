using System.Collections.Generic;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IVehicleService
    {
        Vehicles Add(Vehicles vehicles);

        bool Update(Vehicles vehicles);

        bool Delete(Vehicles vehicles);

        List<Vehicles> GetALL(string mobileNumber);

        Vehicles Get(string mobileNumber);
    }
}