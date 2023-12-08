using System;
using System.Collections.Generic;
using Components.Services.Interfaces;
using Infrastructure.CustomModels;
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
    public class ParkingLotService : IParkingLotService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IChargeSheetRepository _chargeSheetRepository;
        private readonly IChargeSheetPriceRepository _chargeSheetPriceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogging _logger;
        #endregion

        #region Constructor
        public ParkingLotService(IParkingLotRepository parkingLotRepository, IChargeSheetRepository chargeSheetRepository, IChargeSheetPriceRepository chargeSheetPriceRepository,
                            IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogging logging)
        {
            _unitOfWork = unitOfWork;
            _parkingLotRepository = parkingLotRepository;
            _parkingLotRepository.UnitOfWork = unitOfWork;
            _chargeSheetRepository = chargeSheetRepository;
            _chargeSheetRepository.UnitOfWork = unitOfWork;
            _chargeSheetPriceRepository = chargeSheetPriceRepository;
            _chargeSheetPriceRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logging;
        }
        #endregion

        #region Public Methods
        public ParkingLots Get(int id)
        {
            _unitOfWork.Open();
            ParkingLots parkingLot = null;
            try
            {
                _logger.Debug(string.Format("Getting the parking lot with the id: {0}", id));
                parkingLot = _parkingLotRepository.Get(id);
                if (parkingLot.IsNotNull())
                {
                    if (parkingLot.ChargeSheet.IsNull())
                    {
                        _logger.Debug(string.Format("Getting the chargesheet with the parkinglotId: {0}", id));
                        parkingLot.ChargeSheet = _chargeSheetRepository.GetByParkingLotId(id);
                    }
                    else
                    {
                        throw new Park4YouException(ErrorMessages.CHARGE_SHEET_DOES_NOT_EXIST);
                    }

                    if (parkingLot.ChargeSheetPrices.IsNull() || parkingLot.ChargeSheetPrices.Count == 0)
                    {
                        _logger.Debug(string.Format("Getting the chargesheet prices with the chargesheetId: {0}", id));
                        parkingLot.ChargeSheetPrices = _chargeSheetPriceRepository.Get(parkingLot.ChargeSheet.Id);
                    }
                    else
                    {
                        throw new Park4YouException(ErrorMessages.CHARGE_SHEET_PRICES_DOES_NOT_EXIST);
                    }
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.PARKING_LOT_NOT_EXIST);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parkingLot;
        }

        public List<ParkingLots> Get(string searchString)
        {
            _unitOfWork.Open();
            List<ParkingLots> parkingLotList = new List<ParkingLots>();
            try
            {
                _logger.Debug(string.Format("Getting the parking lot the search string: {0}", searchString));
                List<ParkingLots> dbParkingLots = _parkingLotRepository.Get(searchString);
                foreach (ParkingLots item in dbParkingLots)
                {
                    _logger.Debug(string.Format("Getting the chargesheet with the parkinglotId: {0}", item.Id));
                    ChargeSheets chargeSheet = _chargeSheetRepository.GetByParkingLotId(item.Id);
                    if (chargeSheet.IsNotNull())
                    {
                        _logger.Debug(string.Format("Getting the chargesheet prices with the chargesheetid: {0}", chargeSheet.Id));
                        item.ChargeSheetPrices = _chargeSheetPriceRepository.Get(chargeSheet.Id);
                        parkingLotList.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parkingLotList;
        }

        public List<ParkingLotInformation> GetALL()
        {
            _unitOfWork.Open();
            try
            {
                return _parkingLotRepository.GetALL();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
        }

        public List<ParkingLotInformation> GetParkingLots(string userId)
        {
            _unitOfWork.Open();
            try
            {
                return _parkingLotRepository.GetALL(userId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
        }

        public bool Update(ParkingLots parkingLot)
        {
            bool IsUpdated = false;
            _unitOfWork.Open();
            try
            {
                parkingLot.UpdatedOn = DateTime.UtcNow;
                parkingLot.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                _unitOfWork.BeginTransaction();
                _logger.Debug(string.Format("Updating the parkinglot information with the id: {0}", parkingLot.Id));
                IsUpdated = _parkingLotRepository.Update(parkingLot);
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

        public bool Delete(int id)
        {
            bool IsDeleted = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting the parking lot with the id: {0}", id));
                ParkingLots parkingLot = _parkingLotRepository.Get(id);
                if (parkingLot.IsNull())
                    throw new Park4YouException(ErrorMessages.PARKING_LOT_NOT_EXIST);

                _unitOfWork.BeginTransaction();
                parkingLot.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                _logger.Debug(string.Format("Deleting parking lot with the id: {0}", id));
                IsDeleted = _parkingLotRepository.Delete(parkingLot);
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

        public bool Save(ParkingLots parkingLot, ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices)
        {
            bool IsSaved = false;
            _unitOfWork.Open();
            ParkingLots dbParkingLot = null;
            try
            {
                _unitOfWork.BeginTransaction();

                #region ParkingLot
                parkingLot.CreatedOn = DateTime.UtcNow;
                parkingLot.UpdatedOn = DateTime.UtcNow;
                parkingLot.IsDeleted = false;
                parkingLot.UserId = _httpContextAccessor.GetCurrentUserId();
                parkingLot.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                _logger.Debug("Adding new parking lot in the system.");
                dbParkingLot = _parkingLotRepository.Insert(parkingLot);
                #endregion

                #region ChargeSheet
                chargeSheet.ParkingLotId = dbParkingLot.Id;
                chargeSheet.CreatedOn = DateTime.UtcNow;
                chargeSheet.UpdatedOn = DateTime.UtcNow;
                chargeSheet.IsDeleted = false;
                chargeSheet.CreatedBy = _httpContextAccessor.GetCurrentUserId();
                chargeSheet.UserId = _httpContextAccessor.GetCurrentUserId();
                chargeSheet.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                _logger.Debug("Adding new chargesheet in the system.");
                ChargeSheets dbChargeSheet = _chargeSheetRepository.Add(chargeSheet);
                #endregion

                #region ChargeSheetPrices
                foreach (ChargeSheetPrices item in chargeSheetPrices)
                {
                    item.CreatedOn = DateTime.UtcNow;
                    item.UpdatedOn = DateTime.UtcNow;
                    item.ChargeSheetId = dbChargeSheet.Id;
                    item.ChargeSheetRuleId = _GetChargeSheetRuleId(item.AttributeKey);
                }
                _logger.Debug("Adding new chargesheet prices in the system.");
                IsSaved = _chargeSheetPriceRepository.Add(chargeSheetPrices);
                #endregion

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
            return IsSaved;
        }
        #endregion

        #region Private Methods
        private int _GetChargeSheetRuleId(string AttributeKey)
        {
            int ruleId = (int)ChargeSheetRule.NONE;
            if (AttributeKey.StartsWith("BASE"))
                ruleId = (int)ChargeSheetRule.BASE_PRICE;
            else if (AttributeKey.StartsWith("HOURS_BASED"))
                ruleId = (int)ChargeSheetRule.HOURLY_CHANGE;
            else if(AttributeKey.StartsWith("DURATION_BASED"))
                ruleId = (int)ChargeSheetRule.DURATION_CHANGE;
            else if (AttributeKey.StartsWith("EXCEPTION_DATES"))
                ruleId = (int)ChargeSheetRule.EXCEPTIONS;
            else if (AttributeKey.StartsWith("HOURLY_CHANGE"))
                ruleId = (int)ChargeSheetRule.HOURLY_CHANGE;
            else if (AttributeKey.StartsWith("MAXIMUM_PRICE"))
                ruleId = (int)ChargeSheetRule.MAXIMUM_PRICES;            

            return ruleId;
        }
        #endregion
    }
}