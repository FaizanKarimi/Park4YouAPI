using System;
using System.Collections.Generic;
using Components.Services.Interfaces;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IUserCardRepository _userCardRepository;
        private readonly IParkingRepository _parkingRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository, IUserProfileRepository userProfileRepository, ILogging logging,
            IUserSettingRepository userSettingRepository, IUserCardRepository userCardRepository, IParkingRepository parkingRepository, IVehicleRepository vehicleRepository,
            IDeviceRepository deviceRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _userRepository.UnitOfWork = unitOfWork;
            _userProfileRepository = userProfileRepository;
            _userProfileRepository.UnitOfWork = unitOfWork;
            _userSettingRepository = userSettingRepository;
            _userSettingRepository.UnitOfWork = unitOfWork;
            _userCardRepository = userCardRepository;
            _userCardRepository.UnitOfWork = unitOfWork;
            _parkingRepository = parkingRepository;
            _parkingRepository.UnitOfWork = unitOfWork;
            _vehicleRepository = vehicleRepository;
            _vehicleRepository.UnitOfWork = unitOfWork;
            _deviceRepository = deviceRepository;
            _deviceRepository.UnitOfWork = unitOfWork;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public bool DeleteAccount(string mobileNumber)
        {
            bool IsDeleted = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug("Getting the user profile information.");
                UserProfiles userProfile = _userProfileRepository.Get(mobileNumber);
                userProfile.MobileNumber = "anonymous";

                _logger.Debug(string.Format("Getting all the user cards information with the mobileNumber: {0}", mobileNumber));
                List<UserCards> userCards = _userCardRepository.GetALL(mobileNumber);
                foreach (UserCards item in userCards)
                {
                    item.IsDeleted = true;
                    item.UpdatedOn = DateTime.UtcNow;
                }

                _unitOfWork.BeginTransaction();

                _logger.Debug(string.Format("Marking the user profile status as deleted: {0}", mobileNumber));
                _userProfileRepository.Delete(userProfile);

                _logger.Debug(string.Format("Deleting the user setting of the user: {0}", mobileNumber));
                _userSettingRepository.Delete(mobileNumber);

                _logger.Debug(string.Format("Marking the status of user cards to deleted: {0}", mobileNumber));
                _userCardRepository.Update(userCards);

                _logger.Debug(string.Format("Marking the status of the vehicle to deleted: {0}", mobileNumber));
                _vehicleRepository.Delete(mobileNumber);

                _logger.Debug(string.Format("Deleting the device with the mobileNumber: {0}", mobileNumber));
                _deviceRepository.Delete(mobileNumber);

                _logger.Debug(string.Format("Marking the status of parkins to deleted: {0}", mobileNumber));
                _parkingRepository.Delete(mobileNumber);

                _unitOfWork.CommitTransaction();
                IsDeleted = true;
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

        public List<VendorInformation> GetVendors()
        {
            List<VendorInformation> vendors = null;
            _unitOfWork.Open();
            try
            {
                vendors = _userRepository.GetVendors();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return vendors;
        }

        public bool Update(AspNetUsers aspNetUsers)
        {
            _unitOfWork.Open();
            bool IsUpdated = false;
            try
            {
                _unitOfWork.BeginTransaction();
                IsUpdated = _userRepository.Update(aspNetUsers);
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

        public bool VerifyAccount(string mobileNumber, string verificationCode)
        {
            _unitOfWork.Open();
            bool IsUpdated = false;
            try
            {
                _logger.Debug("Getting the user profile information");
                UserProfiles userProfile = _userProfileRepository.Get(mobileNumber);
                if (userProfile.IsNotNull())
                {
                    if (userProfile.VerificationCode.Equals(verificationCode))
                    {
                        _unitOfWork.BeginTransaction();
                        AspNetUsers aspNetUsers = new AspNetUsers()
                        {
                            MobileNumber = mobileNumber,
                            PhoneNumberConfirmed = true
                        };
                        _logger.Debug("Updating the user infomation in the system.");
                        IsUpdated = _userRepository.Update(aspNetUsers);
                        IsUpdated = true;
                        _unitOfWork.CommitTransaction();
                    }
                    else
                        throw new Park4YouException(ErrorMessages.USER_INVALID_VERIFICATION_CODE);
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);
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