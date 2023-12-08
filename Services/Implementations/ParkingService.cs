using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Repository.Provider;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ParkingService : IParkingService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParkingRepository _parkingRepository;
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly IChargeSheetPriceRepository _chargeSheetPriceRepository;
        private readonly IChargeSheetRepository _chargeSheetRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IParkingNotificationRepository _parkingNotificationRepository;
        #endregion

        #region Constructor
        public ParkingService(IParkingRepository parkingRepository, IUnitOfWork unitOfWork, IPaymentOrderRepository paymentOrderRepository, IHttpContextAccessor httpContextAccessor,
            IParkingNotificationRepository parkingNotificationRepository, IChargeSheetPriceRepository chargeSheetPriceRepository, IChargeSheetRepository chargeSheetRepository)
        {
            _unitOfWork = unitOfWork;
            _parkingRepository = parkingRepository;
            _parkingRepository.UnitOfWork = unitOfWork;
            _paymentOrderRepository = paymentOrderRepository;
            _paymentOrderRepository.UnitOfWork = unitOfWork;
            _parkingNotificationRepository = parkingNotificationRepository;
            _parkingNotificationRepository.UnitOfWork = unitOfWork;
            _chargeSheetPriceRepository = chargeSheetPriceRepository;
            _chargeSheetPriceRepository.UnitOfWork = unitOfWork;
            _chargeSheetRepository = chargeSheetRepository;
            _chargeSheetRepository.UnitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public Methods
        public Parkings StartParking(Parkings parking, ParkingNotifications parkingNotifications)
        {
            _unitOfWork.Open();
            try
            {
                ChargeSheets chargeSheet = _chargeSheetRepository.GetByParkingLotId(parking.ParkingLotId);
                List<ChargeSheetPrices> chargeSheetPrices = _chargeSheetPriceRepository.Get(chargeSheet.Id);

                parking.UserId = _httpContextAccessor.GetCurrentUserId();
                _unitOfWork.BeginTransaction();

                #region Parkings
                parking = _parkingRepository.Add(parking);
                parking = _parkingRepository.Get(parking.Id);
                #endregion

                #region Parking Notifications
                parkingNotifications.UserId = parking.UserId;
                parkingNotifications.ParkingId = parking.Id;
                parkingNotifications.CreatedOn = DateTime.UtcNow;
                _parkingNotificationRepository.Add(parkingNotifications);
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
            return parking;
        }

        public Parkings GetParkingDetails(string registrationNumber)
        {
            Parkings parking = null;
            _unitOfWork.Open();
            try
            {
                parking = _parkingRepository.GetParking(registrationNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parking;
        }

        public Parkings Get(int parkingId)
        {
            Parkings parking = null;
            _unitOfWork.Open();
            try
            {
                parking = _parkingRepository.Get(parkingId);
                if (parking.IsNotNull())
                {
                    parking.ParkingNotifications = _parkingNotificationRepository.Get(parking.Id);
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
            return parking;
        }

        public Parkings GetStartParking(int parkingId)
        {
            Parkings parking = null;
            _unitOfWork.Open();
            try
            {
                parking = _parkingRepository.GetStartParking(parkingId, (int)ParkingStatus.START);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parking;
        }

        public Parkings GetStartedParking(string userId)
        {
            Parkings parking = null;
            _unitOfWork.Open();
            try
            {
                parking = _parkingRepository.GetStartedParking(userId, (int)ParkingStatus.START);
                if (parking.IsNotNull())
                    parking.ParkingNotifications = _parkingNotificationRepository.Get(parking.Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parking;
        }

        public Parkings GetStartedParkingByMobileNumber(string mobileNumber)
        {
            Parkings parking = null;
            _unitOfWork.Open();
            try
            {
                parking = _parkingRepository.GetStartParkingByMobileNumber(mobileNumber, (int)ParkingStatus.START);
                if (parking.IsNotNull())
                    parking.ParkingNotifications = _parkingNotificationRepository.Get(parking.Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parking;
        }

        public bool SaveParkingNote(int parkingId, string parkingNote)
        {
            bool IsSaved = false;
            _unitOfWork.Open();
            try
            {
                Parkings dbParking = _parkingRepository.Get(parkingId);
                dbParking.ParkingNote = parkingNote;
                _unitOfWork.BeginTransaction();
                _parkingRepository.Update(dbParking);
                _unitOfWork.CommitTransaction();
                IsSaved = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return IsSaved;
        }

        public bool StopParking(Parkings parking, PaymentOrders paymentOrder)
        {
            bool IsStopped = false;
            _unitOfWork.Open();
            try
            {
                _unitOfWork.BeginTransaction();
                IsStopped = _parkingRepository.Update(parking);
                if (paymentOrder.IsNotNull())
                    _paymentOrderRepository.Add(paymentOrder);
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
            return IsStopped;
        }

        public ParkingReport GetParkingReport(int parkingId)
        {
            _unitOfWork.Open();
            ParkingReport parkingReport = null;
            try
            {
                parkingReport = _parkingRepository.GetParkingReport(parkingId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parkingReport;
        }

        public List<ParkingHistory> GetParkingHistory(string mobileNumber)
        {
            _unitOfWork.Open();
            List<ParkingHistory> parkingHistories = null;
            try
            {
                parkingHistories = _parkingRepository.GetParkingHistory(mobileNumber);
                foreach (ParkingHistory item in parkingHistories)
                {
                    item.RemainingMinutes = Convert.ToInt32(CommonHelpers.GetTotalMinutes(Convert.ToDateTime(item.StopTime), DateTime.UtcNow));
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
            return parkingHistories;
        }

        public List<ParkingAroundAreas> GetParkingAroundAreas(string latitude, string longitude)
        {
            _unitOfWork.Open();
            List<ParkingAroundAreas> parkingAroundAreas = null;
            try
            {
                parkingAroundAreas = _parkingRepository.GetParkingAroundAreas(latitude, longitude);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parkingAroundAreas;
        }

        public Parkings Update(Parkings parking, ParkingNotifications parkingNotifications)
        {
            Parkings dbParking = null;
            _unitOfWork.Open();
            try
            {
                _unitOfWork.BeginTransaction();
                _parkingRepository.Update(parking);
                _parkingNotificationRepository.Update(parkingNotifications);
                _unitOfWork.CommitTransaction();

                dbParking = _parkingRepository.Get(parking.Id);
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
            return dbParking;
        }

        public string GetParkingPushNotificationMessage(Parkings parking)
        {
            string message = string.Empty;
            message = string.Concat("Your parking time for vehicle ", parking.RegistrationNumber, " is about to End Place ", parking.Name, ", please extend the parking or get your vehicle to avoid inconvenience.");
            return message;
        }

        public List<Parkings> GetStartedParkings()
        {
            _unitOfWork.Open();
            List<Parkings> parkings = null;
            try
            {
                parkings = _parkingRepository.GetStartedParkings((int)ParkingStatus.START);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return parkings;
        }
        #endregion
    }
}