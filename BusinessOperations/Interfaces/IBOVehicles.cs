using System.Collections.Generic;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOVehicles
    {
        Vehicles Add(Vehicles vehicles);

        bool Update(Vehicles vehicles);

        bool Delete(Vehicles vehicles);

        List<Vehicles> GetALL(string mobileNumber);
    }
}