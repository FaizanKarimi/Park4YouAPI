using System;
using System.Collections.Generic;
using System.Linq;
using BusinessOperations.Interfaces;
using Components.Services.Interfaces;
using Components.Services.Price;
using Infrastructure.APIResponses.Parking;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace BusinessOperations.Implementations
{
    public class BOParkingLot : IBOParkingLot
    {
        #region Private Members
        private readonly IParkingLotService _parkingLotService;
        private readonly IUserCardService _userCardService;
        private readonly IParkingService _parkingService;
        private readonly ILogging _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserProfileService _userProfileService;
        #endregion

        #region Constructor
        public BOParkingLot(IParkingLotService parkingLotService, IUserCardService userCardService, IParkingService parkingService, ILogging logging,
            IHttpContextAccessor httpContextAccessor, IUserProfileService userProfileService)
        {
            _parkingLotService = parkingLotService;
            _userCardService = userCardService;
            _parkingService = parkingService;
            _logger = logging;
            _httpContextAccessor = httpContextAccessor;
            _userProfileService = userProfileService;
        }
        #endregion

        #region Public Methods
        public ParkingLots Get(int id)
        {
            return _parkingLotService.Get(id);
        }

        public List<ParkingLotModel> Get(string searchString)
        {
            _logger.Debug("Search parking process started.");
            List<ParkingLotModel> parkingLotModelList = new List<ParkingLotModel>();
            _logger.Debug(string.Format("Getting the parking lot with the search string: {0}", searchString));
            List<ParkingLots> parkingLots = _parkingLotService.Get(searchString);
            string userId = _httpContextAccessor.GetCurrentUserId();
            _logger.Debug(string.Format("Getting the user profile information with the userId: {0}", userId));
            UserProfiles userProfile = _userProfileService.GetByUserId(userId);

            foreach (ParkingLots item in parkingLots)
            {
                decimal convertedPrice = Convert.ToDecimal(CommonHelpers.FormattedPriceToDecimal(item.ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue));
                ParkingLotModel parkingLotModel = new ParkingLotModel();
                List<LongitudeLatitudeModel> longitudeLatitudeModelList = new List<LongitudeLatitudeModel>();

                parkingLotModel.AreaCode = item.AreaCode;
                parkingLotModel.CenterPoint = item.CenterCoordinates;

                parkingLotModel.Name = item.Name;
                parkingLotModel.ParkingLotId = item.Id;
                parkingLotModel.BasePrice = convertedPrice == 0.0m ? "Free" : CommonHelpers.ConvertedAmount(convertedPrice, userProfile?.Country);
                if (item.GeoCoordinates != null)
                {
                    string geoCoordinates = item.GeoCoordinates.TrimEnd();
                    string[] geoCoordinatesArray = geoCoordinates.Split(' ');
                    for (int i = 0; i < geoCoordinatesArray.Length; i++)
                    {
                        LongitudeLatitudeModel longitudeLatitudeModel = new LongitudeLatitudeModel();
                        string areas = geoCoordinatesArray[i];
                        string[] ss = areas.Split(',');
                        longitudeLatitudeModel.Latitude = ss[0].ToString();
                        longitudeLatitudeModel.Longitude = ss[1].ToString();
                        longitudeLatitudeModelList.Add(longitudeLatitudeModel);
                    }
                }
                parkingLotModel.AreaGeoCoordinates = longitudeLatitudeModelList.ToList();
                parkingLotModelList.Add(parkingLotModel);
            }
            _logger.Debug("Search parking process ended.");
            return parkingLotModelList;
        }

        public bool Delete(int id)
        {
            _logger.Debug("Delete parking lot process started.");
            bool IsDeleted = false;
            IsDeleted = _parkingLotService.Delete(id);
            _logger.Debug("Delete parking lot process ended.");
            return IsDeleted;
        }

        public List<ParkingLotInformation> GetALL(string userId, string role)
        {
            if (role.Equals(UserRoles.Admin.ToString()))
                return _parkingLotService.GetALL();
            else
                return _parkingLotService.GetParkingLots(userId);
        }

        public string GetParkingPrice(int parkingLotId, int minutes)
        {
            _logger.Debug("Parking price process started.");
            decimal price = 0M;
            DateTime startTime = DateTime.UtcNow;
            DateTime endTime = DateTime.UtcNow.AddMinutes(minutes);
            string userId = _httpContextAccessor.GetCurrentUserId();
            _logger.Debug(string.Format("Getting parkinglot with the id: {0}", parkingLotId));
            ParkingLots parkingLot = _parkingLotService.Get(parkingLotId);

            _logger.Debug(string.Format("Getting the user profile information with the userId: {0}", userId));
            UserProfiles userProfile = _userProfileService.GetByUserId(userId);
            _logger.Debug(string.Format("User country in the user profile is: {0}", userProfile?.Country));
            price = PriceService.CalculatePrice(startTime, endTime, parkingLot.ChargeSheetPrices);
            string convertedPrice = CommonHelpers.ConvertedAmount(price, userProfile?.Country);
            _logger.Debug(string.Format("Converted price is: {0}", convertedPrice));
            _logger.Debug("Parking price process ended.");
            return convertedPrice;
        }

        public bool Update(ParkingLots parkingLot)
        {
            return _parkingLotService.Update(parkingLot);
        }

        public bool Save(ParkingLots parkingLot, ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrice)
        {
            return _parkingLotService.Save(parkingLot, chargeSheet, chargeSheetPrice);
        }
        #endregion
    }
}