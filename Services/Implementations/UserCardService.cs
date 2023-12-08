using System;
using System.Collections.Generic;
using Components.Services.Interfaces;
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
    public class UserCardService : IUserCardService
    {
        #region Private Members
        private readonly IUserCardRepository _userCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IParkingRepository _parkingRepository;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public UserCardService(IUserCardRepository userCardRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogging logging, IParkingRepository parkingRepository)
        {
            _unitOfWork = unitOfWork;
            _userCardRepository = userCardRepository;
            _userCardRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _parkingRepository = parkingRepository;
            _parkingRepository.UnitOfWork = unitOfWork;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public UserCards Add(UserCards newUserCards)
        {
            _unitOfWork.Open();
            UserCards userCard = null;
            List<UserCards> userCards = null;
            try
            {
                if (newUserCards.IsDefault)
                {
                    _logger.Debug(string.Format("Getting all the user cards with the mobileNumber: {0}", newUserCards.MobileNumber));
                    userCards = _userCardRepository.GetALL(newUserCards.MobileNumber);
                    foreach (var item in userCards)
                    {
                        item.IsDefault = false;
                        item.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                    }
                }

                newUserCards.IsDeleted = false;
                newUserCards.CreatedOn = DateTime.UtcNow;
                newUserCards.UpdatedOn = DateTime.UtcNow;
                newUserCards.UserId = _httpContextAccessor.GetCurrentUserId();
                _unitOfWork.BeginTransaction();

                if (userCards.IsNotNull() && userCards.Count > 0)
                {
                    _logger.Debug("Updating the user cards");
                    _userCardRepository.Update(userCards);
                }

                userCard = _userCardRepository.Add(newUserCards);
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
            return userCard;
        }

        public bool Delete(UserCards userCards)
        {
            _unitOfWork.Open();
            bool IsDeleted = false;
            try
            {
                _logger.Debug(string.Format("Getting the started parking of the user with the mobileNumber: {0}, cardId: {1}", userCards.MobileNumber, userCards.Id));
                Parkings dbParking = _parkingRepository.GetStartParkingByMobileNumber(userCards.MobileNumber, (int)ParkingStatus.START, userCards.Id);
                if (dbParking.IsNotNull())
                    throw new Park4YouException(ErrorMessages.USER_CARD_CANNOT_BE_DELETED_BECAUSE_USED_IN_PARKING);

                _logger.Debug(string.Format("Getting the user card information with the cardId: {0}", userCards.Id));
                UserCards dbUserCard = _userCardRepository.Get(userCards.Id);
                if (dbUserCard.IsNotNull())
                {
                    _unitOfWork.BeginTransaction();
                    dbUserCard.IsDeleted = true;
                    dbUserCard.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                    dbUserCard.UpdatedOn = DateTime.UtcNow;
                    _logger.Debug(string.Format("Marking the user card information to deleted with the cardId: {0}", userCards.Id));
                    IsDeleted = _userCardRepository.Delete(dbUserCard);
                    _unitOfWork.CommitTransaction();
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);
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

        public UserCards Get(int CardId)
        {
            UserCards userCard = null;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting the user card with the cardId: {0}", CardId));
                userCard = _userCardRepository.Get(CardId);
                if (userCard.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userCard;
        }

        public List<UserCards> GetALL(string mobileNumber)
        {
            _unitOfWork.Open();
            List<UserCards> userCardList = null;
            try
            {
                userCardList = _userCardRepository.GetALL(mobileNumber);
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
            return userCardList;
        }

        public bool SetDefaultUserCard(int cardId, string mobileNumber)
        {
            bool IsSetToDefault = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting user card information with the cardId: {0}", cardId));
                UserCards userCard = _userCardRepository.Get(cardId);
                if (userCard.IsNotNull())
                {
                    _logger.Debug(string.Format("Getting all the user cards with the mobileNumber: {0}", mobileNumber));
                    List<UserCards> userCards = _userCardRepository.GetALL(mobileNumber);
                    foreach (UserCards item in userCards)
                    {
                        item.IsDefault = false;
                        item.UpdatedOn = DateTime.UtcNow;
                        item.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                    }
                    userCard.IsDefault = true;
                    userCard.UpdatedOn = DateTime.UtcNow;
                    userCard.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                    _unitOfWork.BeginTransaction();
                    _logger.Debug("Updating the user cards.");
                    _userCardRepository.Update(userCards);
                    _logger.Debug("Updating the user card.");
                    _userCardRepository.Update(userCard);
                    _unitOfWork.CommitTransaction();
                    IsSetToDefault = true;
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.USER_CARD_NOT_EXIST);
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
            return IsSetToDefault;
        }
        #endregion
    }
}