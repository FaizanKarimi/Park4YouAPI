using System;
using System.Collections.Generic;
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
    public class ChargeSheetService : IChargeSheetService
    {
        #region Private Methods
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChargeSheetRepository _chargeSheetRepository;
        private readonly IChargeSheetPriceRepository _chargeSheetPriceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public ChargeSheetService(IUnitOfWork unitOfWork, IChargeSheetRepository chargeSheetRepository, IChargeSheetPriceRepository chargeSheetPriceRepository, 
            IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _chargeSheetRepository = chargeSheetRepository;
            _chargeSheetRepository.UnitOfWork = unitOfWork;
            _chargeSheetPriceRepository = chargeSheetPriceRepository;
            _chargeSheetPriceRepository.UnitOfWork = unitOfWork;
            _userRepository = userRepository;
            _userRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public Methods
        public ChargeSheets Get(int id)
        {
            _unitOfWork.Open();
            ChargeSheets chargeSheet = null;
            try
            {
                chargeSheet = _chargeSheetRepository.Get(id);
                if (chargeSheet.IsNull())
                    throw new Park4YouException(ErrorMessages.CHARGE_SHEET_DOES_NOT_EXIST);

                chargeSheet.OwnerName = _userRepository.GetUser(chargeSheet.UserId).UserName;
                chargeSheet.ChargeSheetPrices = _chargeSheetPriceRepository.Get(chargeSheet.Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return chargeSheet;
        }

        public bool Update(ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices)
        {
            bool IsUpdated = false;
            _unitOfWork.Open();
            try
            {
                _unitOfWork.BeginTransaction();
                chargeSheet.UpdatedOn = DateTime.UtcNow;
                chargeSheet.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                IsUpdated = _chargeSheetRepository.Update(chargeSheet);

                foreach (var item in chargeSheetPrices)
                {
                    item.UpdatedOn = DateTime.UtcNow;
                }

                IsUpdated = _chargeSheetPriceRepository.Update(chargeSheetPrices);
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