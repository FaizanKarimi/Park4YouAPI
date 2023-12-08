using System;
using Infrastructure.DataModels;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class DeviceService : IDeviceService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public DeviceService(IUnitOfWork unitOfWork, IDeviceRepository deviceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _deviceRepository = deviceRepository;
            _deviceRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public Methods
        public bool Add(Devices devices)
        {
            _unitOfWork.Open();
            bool IsAdded = false;
            try
            {
                devices.UserId = _httpContextAccessor.GetCurrentUserId();
                IsAdded = _deviceRepository.Add(devices);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return IsAdded;
        }

        public bool Update(Devices devices)
        {
            _unitOfWork.Open();
            bool IsUpdated = false;
            try
            {
                IsUpdated = _deviceRepository.Update(devices);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return IsUpdated;
        }

        public Devices Get(string mobileNumber)
        {
            _unitOfWork.Open();
            Devices device = null;
            try
            {
                device = _deviceRepository.Get(mobileNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return device;
        }

        public Devices GetByUserId(string userId)
        {
            _unitOfWork.Open();
            Devices device = null;
            try
            {
                device = _deviceRepository.GetByUserId(userId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return device;
        }
        #endregion
    }
}