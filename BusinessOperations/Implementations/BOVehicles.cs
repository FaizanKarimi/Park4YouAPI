using System.Collections.Generic;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOVehicles : IBOVehicles
    {
        #region Private Members
        private readonly IVehicleService _vehicleService;
        #endregion

        #region Constructor
        public BOVehicles(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        #endregion

        #region Public Methods
        public Vehicles Add(Vehicles vehicles)
        {
            return _vehicleService.Add(vehicles);
        }

        public bool Delete(Vehicles vehicles)
        {
            return _vehicleService.Delete(vehicles);
        }

        public List<Vehicles> GetALL(string mobileNumber)
        {
            return _vehicleService.GetALL(mobileNumber);
        }

        public bool Update(Vehicles vehicles)
        {
            return _vehicleService.Update(vehicles);
        } 
        #endregion
    }
}