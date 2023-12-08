using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Components.Services.Interfaces;
using Components.Services.PRS.Models;
using Infrastructure.Cache;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Components.Services.PRS
{
    public class PRSService : IPRSService
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogging _logging;
        #endregion

        #region Constructor
        public PRSService(IOptions<AppSettings> options, ILogging logging)
        {
            _appSettings = options;
            _logging = logging;
        }
        #endregion

        #region Public Methods
        public void GetParking(string parkingRightId)
        {
            string url = string.Concat(_appSettings.Value.PRSParkingUrl, parkingRightId);
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Clear();
                webClient.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                webClient.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                webClient.Headers.Add(Constants.ProviderIdParameter, _appSettings.Value.PRSProviderId);
                Uri myUri = new Uri(url, UriKind.Absolute);
                string json = webClient.DownloadString(myUri);
                ParkingRight parkingRight = JsonConvert.DeserializeObject<ParkingRight>(json);
            }
        }

        public string StartParking(string transactionId, string productDescription, string sellingPointId, string sellingPointLocation, string areaManagerId,
            string areaId, string vehicleId, DateTime validityBegin, DateTime validityEnd, string validityHours, bool validityCancelled)
        {
            ParkingRight newParkingRight = new ParkingRight()
            {
                providerId = _appSettings.Value.PRSProviderId,
                transactionId = transactionId,
                productDescription = productDescription,
                sellingPointId = sellingPointId,
                sellingPointLocation = sellingPointLocation,
                areaManagerId = areaManagerId,
                areaId = areaId,
                vehicleId = vehicleId,
                normalizedVehicleId = vehicleId.ToUpper(),
                validityBegin = validityBegin.ToString(Constants.PRSDateTimeFormat),
                validityEnd = validityEnd.ToString(Constants.PRSDateTimeFormat),
                validityHours = validityHours,
                validityCancelled = validityCancelled
            };

            ParkingRight parkingRight = null;
            string url = _appSettings.Value.PRSParkingUrl;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Clear();
                    client.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                    client.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                    client.Headers.Add(Constants.ProviderIdParameter, _appSettings.Value.PRSProviderId);
                    string data = JsonConvert.SerializeObject(newParkingRight);
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    Uri myUri = new Uri(url, UriKind.Absolute);
                    byte[] response = client.UploadData(myUri, HttpMethod.Post.ToString(), byteArray);
                    string json = Encoding.Default.GetString(response);
                    parkingRight = JsonConvert.DeserializeObject<ParkingRight>(json);
                }
                return parkingRight.prid;
            }
            catch (Exception ex)
            {
                _logging.Debug(string.Format("Exception is: {0}", ex));
                throw new Park4YouException(ErrorMessages.PRS_PARKING_REQUEST_FAILED);
            }
        }

        public void UpdateParking(string parkingRightId, string transactionId, string productDescription, string sellingPointId, string sellingPointLocation,
            string areaManagerId, string areaId, string vehicleId, DateTime validityBegin, DateTime validityEnd, string validityHours, bool validityCancelled)
        {
            ParkingRight updatedParkingRight = new ParkingRight()
            {
                prid = parkingRightId,
                providerId = _appSettings.Value.PRSProviderId,
                transactionId = transactionId,
                productDescription = productDescription,
                sellingPointId = sellingPointId,
                sellingPointLocation = sellingPointLocation,
                areaManagerId = areaManagerId,
                areaId = areaId,
                vehicleId = vehicleId,
                normalizedVehicleId = vehicleId.ToUpper(),
                validityEnd = validityEnd.ToString(Constants.PRSDateTimeFormat),
                validityBegin = validityBegin.ToString(Constants.PRSDateTimeFormat),
                validityCancelled = validityCancelled,
                validityHours = validityHours
            };

            string url = string.Concat(_appSettings.Value.PRSParkingUrl, parkingRightId);
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Clear();
                    client.Headers.Add(HttpRequestHeader.Authorization, _GetToken());
                    client.Headers.Add(HttpRequestHeader.ContentType, Constants.ContentType);
                    client.Headers.Add(Constants.ProviderIdParameter, _appSettings.Value.PRSProviderId);
                    string data = JsonConvert.SerializeObject(updatedParkingRight);
                    Uri myUri = new Uri(url, UriKind.Absolute);
                    string response = client.UploadString(myUri, HttpMethod.Put.ToString(), data);
                    ParkingRight parkingRight = JsonConvert.DeserializeObject<ParkingRight>(response);
                }
            }
            catch (Exception ex)
            {
                _logging.Debug(string.Format("Exception is: {0}", ex));
                throw new Park4YouException(ErrorMessages.PRS_PARKING_UPDATE_REQUEST_FAILED);
            }
        }
        #endregion

        #region Private Methods
        private string _GetToken()
        {
            return string.Concat(Constants.BearerAuthentication, _appSettings.Value.PRSToken);
        }
        #endregion
    }
}