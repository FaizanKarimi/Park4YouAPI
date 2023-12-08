using System;

namespace Components.Services.Solvision
{
    public interface ISolvisionService
    {
        /// <summary>
        /// Start the parking at solvision.
        /// </summary>
        /// <param name="parkingName"></param>
        /// <param name="areaCode"></param>
        /// <param name="registrationNumber"></param>
        /// <param name="startTime"></param>
        void StartParking(string parkingName, string areaCode, string registrationNumber, DateTime startTime);

        /// <summary>
        /// Stop the parking at solvisions.
        /// </summary>
        /// <param name="parkingName"></param>
        /// <param name="areaCode"></param>
        /// <param name="registrationNumber"></param>
        /// <param name="stopTime"></param>
        void StopParking(string parkingName, string areaCode, string registrationNumber, DateTime stopTime);
    }
}