using System;

namespace Components.Services.PRS
{
    public interface IPRSService
    {
        /// <summary>
        /// Start the parking.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="transactionId"></param>
        /// <param name="productDescription"></param>
        /// <param name="sellingPointId"></param>
        /// <param name="sellingPointLocation"></param>
        /// <param name="areaManagerId"></param>
        /// <param name="areaId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="validityBegin"></param>
        /// <param name="validityEnd"></param>
        /// <param name="validityHours"></param>
        /// <param name="validityCancelled"></param>
        string StartParking(string transactionId, string productDescription, string sellingPointId, string sellingPointLocation, string areaManagerId,
            string areaId, string vehicleId, DateTime validityBegin, DateTime validityEnd, string validityHours, bool validityCancelled);

        /// <summary>
        /// Get the parking.
        /// </summary>
        /// <param name="parkingRightId"></param>
        void GetParking(string parkingRightId);

        /// <summary>
        /// Update the parking
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="transactionId"></param>
        /// <param name="productDescription"></param>
        /// <param name="sellingPointId"></param>
        /// <param name="sellingPointLocation"></param>
        /// <param name="areaManagerId"></param>
        /// <param name="areaId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="validityBegin"></param>
        /// <param name="validityEnd"></param>
        /// <param name="validityHours"></param>
        /// <param name="validityCancelled"></param>
        void UpdateParking(string parkingRightId, string transactionId, string productDescription, string sellingPointId, string sellingPointLocation, string areaManagerId,
            string areaId, string vehicleId, DateTime validityBegin, DateTime validityEnd, string validityHours, bool validityCancelled);
    }
}