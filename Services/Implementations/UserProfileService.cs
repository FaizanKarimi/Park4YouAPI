using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserProfileService : IUserProfileService
    {
        #region Private Members
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserCardRepository _userCardRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserAreaRepository _userAreaRepository;
        private readonly IParkingRepository _parkingRepository;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IChargeSheetRepository _chargeSheetRepository;
        private readonly IChargeSheetPriceRepository _chargeSheetPriceRepository;
        private readonly ILogging _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public UserProfileService(IUserProfileRepository userProfileRepository, IDeviceRepository deviceRepository, IUnitOfWork unitOfWork,
            IUserCardRepository userCardRepository, IVehicleRepository vehicleRepository, IUserAreaRepository userAreaRepository, IParkingRepository parkingRepository,
            IParkingLotRepository parkingLotRepository, IUserSettingRepository userSettingRepository, ILogging logging, IHttpContextAccessor httpContextAccessor,
            IChargeSheetRepository chargeSheetRepository, IChargeSheetPriceRepository chargeSheetPriceRepository)
        {
            _unitOfWork = unitOfWork;
            _userProfileRepository = userProfileRepository;
            _userProfileRepository.UnitOfWork = unitOfWork;
            _deviceRepository = deviceRepository;
            deviceRepository.UnitOfWork = unitOfWork;
            _userCardRepository = userCardRepository;
            _userCardRepository.UnitOfWork = unitOfWork;
            _vehicleRepository = vehicleRepository;
            _vehicleRepository.UnitOfWork = unitOfWork;
            _userAreaRepository = userAreaRepository;
            _userAreaRepository.UnitOfWork = unitOfWork;
            _parkingRepository = parkingRepository;
            _parkingRepository.UnitOfWork = unitOfWork;
            _parkingLotRepository = parkingLotRepository;
            _parkingLotRepository.UnitOfWork = unitOfWork;
            _userSettingRepository = userSettingRepository;
            _userSettingRepository.UnitOfWork = unitOfWork;
            _chargeSheetRepository = chargeSheetRepository;
            _chargeSheetRepository.UnitOfWork = unitOfWork;
            _chargeSheetPriceRepository = chargeSheetPriceRepository;
            _chargeSheetPriceRepository.UnitOfWork = unitOfWork;
            _logger = logging;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public Methods
        public UserProfiles Get(string mobileNumber)
        {
            _unitOfWork.Open();
            UserProfiles userProfile = null;
            try
            {
                userProfile = _userProfileRepository.Get(mobileNumber);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userProfile;
        }

        public UserProfiles GetByUserId(string userId)
        {
            _unitOfWork.Open();
            UserProfiles userProfile = null;
            try
            {
                userProfile = _userProfileRepository.GetByUserId(userId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userProfile;
        }

        public bool UpdateVerficationCode(string mobileNumber, string verificationCode)
        {
            bool IsUpdated = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug("Getting the user profile against mobilenumber.");
                UserProfiles UserProfile = _userProfileRepository.Get(mobileNumber);
                if (UserProfile.IsNotNull())
                {
                    UserProfile.VerificationCode = verificationCode;
                    _unitOfWork.BeginTransaction();
                    _logger.Debug("Updating the user profile against mobilenumber.");
                    IsUpdated = _userProfileRepository.Update(UserProfile);
                    _unitOfWork.CommitTransaction();
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);
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
            return IsUpdated;
        }

        public bool AddUserProfile(UserProfiles userProfile, Devices device, string language)
        {
            bool IsAdded = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting the user profile information with the mobilenumber: {0}", userProfile.MobileNumber));
                UserProfiles dbUserProfile = _userProfileRepository.Get(userProfile.MobileNumber);
                if (dbUserProfile.IsNull())
                {
                    _unitOfWork.BeginTransaction();
                    _logger.Debug("Adding new profile in the system.");
                    _userProfileRepository.Add(userProfile);
                    Devices dbDevice = _deviceRepository.Get(userProfile.MobileNumber);
                    if (dbDevice.IsNotNull())
                    {
                        dbDevice.UserId = device.UserId;
                        dbDevice.RegistrationToken = device.RegistrationToken;
                        dbDevice.DeviceToken = device.DeviceToken;
                        dbDevice.UpdatedOn = DateTime.UtcNow;
                        dbDevice.DeviceTypeId = device.DeviceTypeId;
                        _logger.Debug(string.Format("Update the device information with the mobileNumber: {0}", dbDevice.MobileNumber));
                        _deviceRepository.Update(dbDevice);
                    }
                    else
                    {
                        Devices newDevice = new Devices()
                        {
                            UserId = userProfile.UserId,
                            MobileNumber = userProfile.MobileNumber,
                            DeviceToken = device.DeviceToken,
                            RegistrationToken = device.RegistrationToken,
                            DeviceTypeId = device.DeviceTypeId,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        };
                        _logger.Debug("Adding new device information in the system.");
                        _deviceRepository.Add(newDevice);
                    }

                    #region Add Default UserSettings
                    List<UserSettings> userSettings = new List<UserSettings>();
                    UserSettings clockTypeUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.CLOCK_TYPE.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(clockTypeUserSetting);
                    UserSettings recentAreaUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.RECENT_AREA.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(recentAreaUserSetting);
                    UserSettings recentCarUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.RECENT_CAR.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(recentCarUserSetting);
                    UserSettings soundUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.SOUND.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(soundUserSetting);
                    UserSettings fixedNotificationFivteenMinutesUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(fixedNotificationFivteenMinutesUserSetting);
                    UserSettings fixedNotificationThirtyMinutesUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(fixedNotificationThirtyMinutesUserSetting);
                    UserSettings parkingPdfUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.PARKING_PDF_RECEIPT.ToString(),
                        AttributeValue = Flag.FALSE.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(parkingPdfUserSetting);
                    UserSettings languageUserSetting = new UserSettings()
                    {
                        UserId = device.UserId,
                        AttributeKey = UserSetting.LANGUAGE.ToString(),
                        AttributeValue = language,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        MobileNumber = userProfile.MobileNumber
                    };
                    userSettings.Add(languageUserSetting);
                    _logger.Debug("Adding new user settings for the new user.");
                    _userSettingRepository.Add(userSettings);
                    #endregion

                    _unitOfWork.CommitTransaction();
                    IsAdded = true;
                }
                else
                {
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_ALREADY_EXIST);
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
            return IsAdded;
        }

        public bool Update(UserProfiles userProfile)
        {
            bool IsUpdated = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting user profile with the mobile number: {0}", userProfile.MobileNumber));
                UserProfiles dbUserProfile = _userProfileRepository.Get(userProfile.MobileNumber);
                if (dbUserProfile.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                _unitOfWork.BeginTransaction();
                dbUserProfile.StreetNumber = userProfile.StreetNumber;
                dbUserProfile.Town = userProfile.Town;
                dbUserProfile.ZipCode = userProfile.ZipCode;
                dbUserProfile.EmailAddress = userProfile.EmailAddress;
                dbUserProfile.FirstName = userProfile.FirstName;
                dbUserProfile.LastName = userProfile.LastName;
                dbUserProfile.MobileNumber = userProfile.MobileNumber;
                dbUserProfile.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                dbUserProfile.UpdatedOn = DateTime.UtcNow;
                dbUserProfile.Country = userProfile.Country;
                _logger.Debug(string.Format("Updating the user profile with the mobilenumber: {0}", dbUserProfile.MobileNumber));
                IsUpdated = _userProfileRepository.Update(dbUserProfile);
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

        public bool UpdateByUserId(UserProfiles userProfile)
        {
            bool IsUpdated = false;
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting the user profile information with the userId: {0}", userProfile.UserId));
                UserProfiles dbUserProfile = _userProfileRepository.GetByUserId(userProfile.UserId);
                if (dbUserProfile.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                _unitOfWork.BeginTransaction();
                dbUserProfile.StreetNumber = userProfile.StreetNumber;
                dbUserProfile.Town = userProfile.Town;
                dbUserProfile.ZipCode = userProfile.ZipCode;
                dbUserProfile.EmailAddress = userProfile.EmailAddress;
                dbUserProfile.FirstName = userProfile.FirstName;
                dbUserProfile.LastName = userProfile.LastName;
                dbUserProfile.MobileNumber = userProfile.MobileNumber;
                dbUserProfile.UpdatedBy = _httpContextAccessor.GetCurrentUserId();
                dbUserProfile.UpdatedOn = DateTime.UtcNow;
                _logger.Debug(string.Format("Updating the user profile information with the mobileNumber: {0}", dbUserProfile.MobileNumber));
                IsUpdated = _userProfileRepository.Update(dbUserProfile);
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

        public UserProfileInformation GetUserProfileInformation(string mobileNumber)
        {
            UserProfileInformation userProfileInformation = new UserProfileInformation();
            _unitOfWork.Open();
            try
            {
                _logger.Debug(string.Format("Getting the user profile information with the mobileNumber: {0}", mobileNumber));
                UserProfiles userProfile = _userProfileRepository.Get(mobileNumber);
                if (userProfile.IsNull())
                    throw new Park4YouException(ErrorMessages.USER_PROFILE_NOT_EXIST);

                #region User Profile Mapping
                userProfileInformation.MobileNumber = userProfile.MobileNumber;
                userProfileInformation.FirstName = userProfile.FirstName;
                userProfileInformation.LastName = userProfile.LastName;
                userProfileInformation.StreetNumber = userProfile.StreetNumber;
                userProfileInformation.ZipCode = userProfile.ZipCode;
                userProfileInformation.Town = userProfile.Town;
                userProfileInformation.Country = userProfile.Country;
                userProfileInformation.CountryCode = userProfile.CountryCode;
                userProfileInformation.EmailAddress = userProfile.EmailAddress;
                #endregion

                #region User Cards Mapping
                List<UserCardsInformation> userCardsInformations = new List<UserCardsInformation>();
                _logger.Debug(string.Format("Getting all the user cards against mobileNumber: {0}", mobileNumber));
                List<UserCards> userCards = _userCardRepository.GetALL(mobileNumber);
                foreach (UserCards item in userCards)
                {
                    UserCardsInformation userCardsInformation = new UserCardsInformation()
                    {
                        Id = item.Id,
                        CardExpiry = item.CardExpiry,
                        CardNumber = item.CardNumber,
                        CardVerificationValue = item.CardVerficationValue,
                        IsDefault = item.IsDefault,
                        Name = item.Name,
                        PaymentType = item.PaymentType
                    };
                    userCardsInformations.Add(userCardsInformation);
                }
                userProfileInformation.UserCards = userCardsInformations;
                #endregion

                #region User Vehicles Mapping
                List<UserVehicles> userVehicles = new List<UserVehicles>();
                _logger.Debug(string.Format("Getting all the vehicles against the mobileNumber: {0}", mobileNumber));
                List<Vehicles> vehicles = _vehicleRepository.GetALL(mobileNumber);
                foreach (Vehicles item in vehicles)
                {
                    UserVehicles userVehicle = new UserVehicles()
                    {
                        Id = item.Id,
                        IsLatest = item.IsLatest,
                        Name = item.Name,
                        RegistrationId = item.RegistrationId,
                        RegistrationNumber = item.RegistrationNumber
                    };
                    userVehicles.Add(userVehicle);
                }
                userProfileInformation.Vehicles = userVehicles;
                #endregion

                #region User Area Mapping
                List<UserAreaModel> userAreaModels = new List<UserAreaModel>();
                _logger.Debug(string.Format("Getting all the user areas with the mobileNumber: {0}", mobileNumber));
                List<UserAreas> userAreas = _userAreaRepository.GetALL(mobileNumber);
                foreach (UserAreas item in userAreas)
                {
                    UserAreaModel userAreaModel = new UserAreaModel()
                    {
                        Id = item.Id,
                        AreaCode = item.AreaCode,
                        City = item.City,
                        Country = item.Country,
                        IsLatest = item.IsLatest,
                        Town = item.Town
                    };
                    userAreaModels.Add(userAreaModel);
                }
                userProfileInformation.Areas = userAreaModels;
                #endregion

                #region User Settings Mapping
                List<UserSettingInformation> userSettingInformations = new List<UserSettingInformation>();
                _logger.Debug(string.Format("Getting all the user settings with the mobileNumber: {0}", mobileNumber));
                List<UserSettings> userSettings = _userSettingRepository.Get(mobileNumber);
                foreach (UserSettings item in userSettings)
                {
                    UserSettingInformation userSettingInformation = new UserSettingInformation()
                    {
                        Id = item.Id,
                        AttributeKey = item.AttributeKey,
                        AttributeValue = item.AttributeValue
                    };
                    userSettingInformations.Add(userSettingInformation);
                }
                userProfileInformation.UserSettings = userSettingInformations;
                #endregion

                #region User Parking Mapping
                _logger.Debug(string.Format("Getting the current active parking with the mobileNumber: {0}", mobileNumber));
                Parkings parking = _parkingRepository.Get(mobileNumber);
                if (parking.IsNotNull())
                {
                    _logger.Debug(string.Format("Getting the parking lot with the parkinglotId: {0}", parking.ParkingLotId));
                    ParkingLots parkingLot = _parkingLotRepository.Get(parking.ParkingLotId);
                    parkingLot.ChargeSheet = _chargeSheetRepository.GetByParkingLotId(parkingLot.Id);
                    parkingLot.ChargeSheetPrices = _chargeSheetPriceRepository.Get(parkingLot.ChargeSheet.Id);
                    UserStartedParking userParking = new UserStartedParking()
                    {
                        AreaCode = parking.AreaCode,
                        IsFixed = parking.IsFixed,
                        Id = parking.Id,
                        Name = parking.Name,
                        ParkingLotId = parking.ParkingLotId,
                        StartTime = parking.StartTime.ToString(Constants.MobileDateTimeFormat),
                        StopTime = parking.StopTime.ToString(Constants.MobileDateTimeFormat),
                        VehicleName = _vehicleRepository.Get(parking.VehicleId)?.Name,
                        RegistrationNumber = parking.RegistrationNumber,
                        ParkingNote = parking.ParkingNote,
                        RemainingSeconds = CommonHelpers.GetTotalSeconds(parking.StopTime, DateTime.UtcNow) < 0 ? 0 : CommonHelpers.GetTotalSeconds(parking.StopTime, DateTime.UtcNow),
                        TotatSeconds = CommonHelpers.GetTotalSeconds(parking.StopTime, parking.StartTime),
                        TotalPrice = CommonHelpers.ConvertedAmount(parking.Price, userProfile.Country),
                        CenterPoint = parkingLot.CenterCoordinates,
                        BasePrice = CommonHelpers.ConvertedAmount(Convert.ToDecimal(parkingLot.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString()))?.AttributeValue), userProfile.Country)
                    };

                    #region Geo Coordinates Mapping
                    List<LongitudeLatitudeModel> longitudeLatitudeModelList = new List<LongitudeLatitudeModel>();
                    if (!string.IsNullOrEmpty(parkingLot.GeoCoordinates))
                    {
                        string geoCoordinates = parkingLot.GeoCoordinates.TrimEnd();
                        string[] geoCoordinatesArray = geoCoordinates.Split(' ');
                        for (int i = 0; i < geoCoordinatesArray.Length; i++)
                        {
                            LongitudeLatitudeModel longitudeLatitudeModel = new LongitudeLatitudeModel();
                            string areas = geoCoordinatesArray[i];
                            string[] ss = areas.Split(',');
                            longitudeLatitudeModel.Latitude = ss[1].ToString();
                            longitudeLatitudeModel.Longitude = ss[0].ToString();
                            longitudeLatitudeModelList.Add(longitudeLatitudeModel);
                        }
                    }
                    userParking.AreaGeoCoordinates = longitudeLatitudeModelList;
                    #endregion

                    userProfileInformation.Parking = userParking;
                }
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _unitOfWork.Close();
            }
            return userProfileInformation;
        }

        public bool AddUserProfile(UserProfiles userProfile)
        {
            bool IsAdded = false;
            _unitOfWork.Open();
            try
            {
                _unitOfWork.BeginTransaction();
                IsAdded = _userProfileRepository.Add(userProfile);
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
            return IsAdded;
        }
        #endregion
    }
}