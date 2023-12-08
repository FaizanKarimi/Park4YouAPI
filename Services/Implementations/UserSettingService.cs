using System;
using System.Collections.Generic;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserSettingService : IUserSettingService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSettingRepository _userSettingRepository;
        #endregion

        #region Constructor
        public UserSettingService(IUserSettingRepository userSettingRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userSettingRepository = userSettingRepository;
            _userSettingRepository.UnitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public List<UserSettings> Get(string mobileNumber)
        {
            _unitOfWork.Open();
            List<UserSettings> userSettings = null;
            try
            {
                userSettings = _userSettingRepository.Get(mobileNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userSettings;
        }

        public bool Update(UserSettings userSetting)
        {
            _unitOfWork.Open();
            bool IsUpdated = false;
            try
            {
                UserSettings dbUserSetting = _userSettingRepository.Get(userSetting.Id);
                if (dbUserSetting.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_SETTING_NOT_EXIST);

                dbUserSetting.UpdatedOn = DateTime.UtcNow;
                dbUserSetting.AttributeValue = userSetting.AttributeValue.ToUpper();
                _unitOfWork.BeginTransaction();
                _userSettingRepository.Update(dbUserSetting);
                _unitOfWork.CommitTransaction();
                IsUpdated = true;
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