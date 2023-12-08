using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserAreaService : IUserAreaService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAreaRepository _userAreaRepository;
        #endregion

        #region Constructor
        public UserAreaService(IUserAreaRepository userAreaRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userAreaRepository = userAreaRepository;
            _userAreaRepository.UnitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public UserAreas AddUserArea(UserAreas userArea)
        {
            UserAreas userAreas = null;
            _unitOfWork.Open();
            try
            {
                List<UserAreas> dbUserAreas = _userAreaRepository.GetALL(userArea.MobileNumber);
                foreach (UserAreas item in dbUserAreas)
                {
                    item.IsLatest = false;
                }
                userArea.CreatedOn = DateTime.UtcNow;
                userArea.UpdatedOn = DateTime.UtcNow;
                userArea.IsDeleted = false;
                _unitOfWork.BeginTransaction();
                _userAreaRepository.Update(dbUserAreas);

                userAreas = _userAreaRepository.Add(userArea);
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
            return userAreas;
        }

        public bool DeleteArea(int id)
        {
            _unitOfWork.Open();
            bool IsDeleted = false;
            try
            {
                UserAreas userArea = _userAreaRepository.Get(id);
                if (userArea.IsNotNull())
                {
                    _unitOfWork.BeginTransaction();
                    string mobileNumber = userArea.MobileNumber;
                    bool IsLatest = userArea.IsLatest;
                    _userAreaRepository.Delete(id);
                    if (!IsLatest)
                    {
                        UserAreas userAreas = _userAreaRepository.GetALL(mobileNumber).OrderByDescending(n => n.Id).FirstOrDefault();
                        if (userAreas.IsNotNull())
                        {
                            userAreas.IsLatest = true;
                            _userAreaRepository.Update(userArea);
                            IsDeleted = true;                            
                        }
                        //else
                        //{
                        //    _unitOfWork.RollBackTransaction();
                        //    throw new Park4YouException(ErrorMessages.USER_AREA_DOES_NOT_EXIST);
                        //}
                    }
                    //else
                    //{
                    //    _unitOfWork.RollBackTransaction();
                    //    throw new Park4YouException(ErrorMessages.USER_NO_LATEST_AREA_EXIST);
                    //}
                    _unitOfWork.CommitTransaction();
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.USER_AREA_DOES_NOT_EXIST);
                }
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

        public List<UserAreas> GetUserAreas(string mobileNumber)
        {
            List<UserAreas> userAreas = null;
            _unitOfWork.Open();
            try
            {
                userAreas = _userAreaRepository.GetALL(mobileNumber);
                //if (userAreas.IsNotNull() && userAreas.Count == 0)
                //    throw new Park4YouException(ErrorMessages.USER_AREA_DOES_NOT_EXIST);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userAreas;
        }
        #endregion
    }
}