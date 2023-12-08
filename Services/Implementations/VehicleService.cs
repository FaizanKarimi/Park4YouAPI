using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IParkingRepository _parkingRepository;
        #endregion

        #region Constructor
        public VehicleService(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IParkingRepository parkingRepository)
        {
            _unitOfWork = unitOfWork;
            _vehicleRepository = vehicleRepository;
            _vehicleRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _parkingRepository = parkingRepository;
            _parkingRepository.UnitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public Vehicles Add(Vehicles newVehicle)
        {
            _unitOfWork.Open();
            List<Vehicles> vehicleList = null;
            Vehicles vehicles = null;
            try
            {
                vehicleList = _vehicleRepository.GetALL(newVehicle.MobileNumber);
                foreach (Vehicles vehicle in vehicleList)
                {
                    vehicle.IsLatest = false;
                    vehicle.UpdatedOn = DateTime.UtcNow;
                }

                string vehicleRegistartionNumber = vehicleList.FirstOrDefault(x => x.RegistrationNumber.Equals(newVehicle.RegistrationNumber) && x.IsDeleted == false && x.MobileNumber.Equals(newVehicle.MobileNumber))?.RegistrationNumber;
                if (!string.IsNullOrEmpty(vehicleRegistartionNumber))
                    throw new Park4YouException(ErrorMessages.VEHICLE_ALREADY_EXIST);

                _unitOfWork.BeginTransaction();
                newVehicle.IsDeleted = false;
                newVehicle.UpdatedOn = DateTime.UtcNow;
                newVehicle.CreatedOn = DateTime.UtcNow;
                newVehicle.UserId = _httpContextAccessor.GetCurrentUserId();
                _vehicleRepository.Update(vehicleList);
                vehicles = _vehicleRepository.Add(newVehicle);
                _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return vehicles;
        }

        public bool Delete(Vehicles deletedVehicles)
        {
            _unitOfWork.Open();
            bool IsDeleted = false;
            Vehicles vehicles = null;
            try
            {
                vehicles = _vehicleRepository.Get(deletedVehicles.Id);
                if (vehicles.IsNull())
                    throw new Park4YouException(ErrorMessages.VEHICLE_DOES_NOT_EXIST);

                Parkings dbParking = _parkingRepository.GetStartParkingByVehicleId(vehicles.MobileNumber, (int)ParkingStatus.START, vehicles.Id);
                if (dbParking.IsNotNull())
                    throw new Park4YouException(ErrorMessages.VEHICLE_CANNOT_BE_DELETED_BECAUSE_USED_IN_PARKING);

                bool isLatestVehicle = vehicles.IsLatest;
                vehicles.IsDeleted = true;
                _unitOfWork.BeginTransaction();
                IsDeleted = _vehicleRepository.Delete(vehicles);

                if (isLatestVehicle)
                {
                    Vehicles vehicleLatest = _vehicleRepository.GetALL(vehicles.MobileNumber)?.FirstOrDefault();
                    if (vehicleLatest.IsNotNull())
                    {
                        vehicleLatest.IsLatest = true;
                        vehicleLatest.UpdatedOn = DateTime.UtcNow;
                        _vehicleRepository.Update(vehicleLatest);
                    }
                }

                _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return IsDeleted;
        }

        public Vehicles Get(string mobileNumber)
        {
            _unitOfWork.Open();
            Vehicles vehicle = null;
            try
            {
                vehicle = _vehicleRepository.Get(mobileNumber);
                if (vehicle.IsNull())
                    throw new Park4YouException(ErrorMessages.VEHICLE_DOES_NOT_EXIST);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return vehicle;
        }

        public List<Vehicles> GetALL(string mobileNumber)
        {
            _unitOfWork.Open();
            List<Vehicles> vehicleList = null;
            try
            {
                vehicleList = _vehicleRepository.GetALL(mobileNumber);
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return vehicleList;
        }

        public bool Update(Vehicles vehicles)
        {
            _unitOfWork.Open();
            bool IsUpdated = false;
            Vehicles dbVehicleLatest = null;
            try
            {
                Vehicles dbVehicle = _vehicleRepository.Get(vehicles.Id);
                if (dbVehicle.IsNull())
                    throw new Park4YouException(ErrorMessages.VEHICLE_DOES_NOT_EXIST);

                if (vehicles.IsLatest)
                {
                    dbVehicleLatest = _vehicleRepository.GetALL(vehicles.MobileNumber).FirstOrDefault(x => x.IsLatest == true);
                    if (dbVehicleLatest.IsNotNull())
                    {
                        dbVehicleLatest.IsLatest = false;
                    }
                }
                else
                {
                    dbVehicleLatest = _vehicleRepository.GetALL(vehicles.MobileNumber).FirstOrDefault(x => x.Id != vehicles.Id);
                    if (dbVehicleLatest.IsNotNull())
                    {
                        dbVehicleLatest.IsLatest = true;
                    }
                }

                dbVehicle.UpdatedOn = DateTime.UtcNow;
                dbVehicle.RegistrationNumber = vehicles.RegistrationNumber;
                dbVehicle.Name = vehicles.Name;
                dbVehicle.IsLatest = vehicles.IsLatest;
                dbVehicle.IsDeleted = vehicles.IsDeleted;
                dbVehicle.MobileNumber = vehicles.MobileNumber;
                _unitOfWork.BeginTransaction();

                if (dbVehicleLatest.IsNotNull())
                {
                    _vehicleRepository.Update(dbVehicleLatest);
                }

                IsUpdated = _vehicleRepository.Update(dbVehicle);
                _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return IsUpdated;
        }
        #endregion
    }
}