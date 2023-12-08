using System;
using System.Net;
using Components.Services.Solvision.Models;
using Infrastructure.Cache;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Components.Services.Solvision
{
    public class SolvisionService : ISolvisionService
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMemoryCache _memoryCache;
        #endregion

        #region Constructor
        public SolvisionService(IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            _appSettings = appSettings;
            _memoryCache = memoryCache;
        }
        #endregion

        #region Public Methods
        public void StartParking(string parkingName, string areaCode, string registrationNumber, DateTime startTime)
        {
            string hash = string.Empty;
            LoginResponse loginResponse = this._Login();
            if (loginResponse.Status.Equals(RequestStatus.SUCCESS.ToString().ToLower()))
            {
                hash = loginResponse.Hash;
                this._StartParkingRequest(parkingName, areaCode, registrationNumber, startTime, hash);
            }
        }

        public void StopParking(string parkingName, string areaCode, string registrationNumber, DateTime stopTime)
        {
            string hash = string.Empty;
            LoginResponse loginResponse = this._Login();
            if (loginResponse.Status.Equals(RequestStatus.SUCCESS.ToString().ToLower()))
            {
                hash = loginResponse.Hash;
                this._StopParkingRequest(parkingName, areaCode, registrationNumber, stopTime, hash);
            }
        }
        #endregion

        #region Private Methods
        private void _StartParkingRequest(string parkingName, string areaCode, string registrationNumber, DateTime startTime, string hash)
        {
            try
            {
                string json = string.Empty;
                using (WebClient webClient = new WebClient())
                {
                    webClient.BaseAddress = _appSettings.Value.SolvisionBaseAddress;
                    webClient.Headers.Clear();
                    webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);

                    webClient.Headers.Add(Constants.HashParameter, hash);
                    webClient.Headers.Add(Constants.UserNameParameter, _appSettings.Value.SolvisionAPIUserName);

                    ParkingStartRequest parkingStartRequest = new ParkingStartRequest()
                    {
                        Name = parkingName,
                        AreaCode = areaCode,
                        RegNumber = registrationNumber,
                        StartTime = startTime.ToString(Constants.SolvisionDateTimeFormat)
                    };

                    string stringContent = JsonConvert.SerializeObject(parkingStartRequest);
                    json = webClient.UploadString(_appSettings.Value.SolvisionStartParkingUrl, stringContent);
                    ParkingStartResponse parkingStartResponse = JsonConvert.DeserializeObject<ParkingStartResponse>(json);
                    if (!parkingStartResponse.Status.Equals(RequestStatus.SUCCESS.ToString().ToLower()))
                        throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_REQUEST_ERROR);
                }
            }
            catch (Park4YouException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_REQUEST_FAILED);
            }
        }

        private void _StopParkingRequest(string parkingName, string areaCode, string registrationNumber, DateTime stopTime, string hash)
        {
            try
            {
                string json = string.Empty;
                using (WebClient webClient = new WebClient())
                {
                    webClient.BaseAddress = _appSettings.Value.SolvisionBaseAddress;
                    webClient.Headers.Clear();
                    webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);

                    webClient.Headers.Add(Constants.HashParameter, hash);
                    webClient.Headers.Add(Constants.UserNameParameter, _appSettings.Value.SolvisionAPIUserName);

                    ParkingStopRequest parkingStopRequest = new ParkingStopRequest()
                    {
                        Name = parkingName,
                        AreaCode = areaCode,
                        RegNumber = registrationNumber,
                        StopTime = stopTime.ToString(Constants.SolvisionDateTimeFormat)
                    };

                    string stringContent = JsonConvert.SerializeObject(parkingStopRequest);
                    json = webClient.UploadString(_appSettings.Value.SolvisionStopParkingUrl, stringContent);
                    ParkingStopResponse parkingStopResponse = JsonConvert.DeserializeObject<ParkingStopResponse>(json);
                    if (!parkingStopResponse.Status.Equals(RequestStatus.SUCCESS.ToString().ToLower()))
                        throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_REQUEST_ERROR);
                }
            }
            catch (Park4YouException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_REQUEST_FAILED);
            }
        }

        private LoginResponse _Login()
        {
            LoginResponse response = new LoginResponse();
            try
            {
                if (_memoryCache.Get<DateTime>(Park4YouCacheKeys.SolvisionTokenExpiryTime) > DateTime.UtcNow)
                {
                    response.Status = RequestStatus.SUCCESS.ToString().ToLower();
                    response.Hash = _memoryCache.Get<string>(Park4YouCacheKeys.SolvisionTokenHash);
                    return response;
                }

                string json = string.Empty;
                using (WebClient webClient = new WebClient())
                {
                    webClient.BaseAddress = _appSettings.Value.SolvisionBaseAddress;
                    webClient.Headers.Clear();
                    webClient.Headers.Add(HttpRequestHeader.Accept, Constants.Accept);
                    LoginRequest loginRequest = new LoginRequest()
                    {
                        Username = _appSettings.Value.SolvisionAPIUserName,
                        Password = _appSettings.Value.SolvisionAPIPassword
                    };
                    string stringContent = JsonConvert.SerializeObject(loginRequest);
                    json = webClient.UploadString(_appSettings.Value.SolvisionLoginUrl, stringContent);
                    response = JsonConvert.DeserializeObject<LoginResponse>(json);

                    if (response.Status.Equals(RequestStatus.SUCCESS.ToString().ToLower()))
                    {
                        _memoryCache.Set<DateTime>(Park4YouCacheKeys.SolvisionTokenExpiryTime, response.Expire.ToUniversalTime());
                        _memoryCache.Set<string>(Park4YouCacheKeys.SolvisionTokenHash, response.Hash);
                        return response;
                    }
                    else
                        throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_REQUEST_LOGIN_FAILED);
                }
            }
            catch (Park4YouException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.SOLVISION_PARKING_LOGIN_ERROR);
            }
        }
        #endregion
    }
}