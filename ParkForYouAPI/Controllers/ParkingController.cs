using BusinessOperations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.Parking;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        #region Private Members
        private readonly IBOParking _bOParking;
        private readonly IBOParkingLot _bOParkingLot;
        #endregion

        #region Constructor
        public ParkingController(IBOParking bOParking, IBOParkingLot bOParkingLot)
        {
            _bOParking = bOParking;
            _bOParkingLot = bOParkingLot;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("prepaid")]
        public IActionResult StartParking(StartParkingRequest startParkingRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.StartParking(startParkingRequest.ParkingId, startParkingRequest.ParkingLotId, startParkingRequest.CardId, startParkingRequest.MobileNumber,
                startParkingRequest.RegistrationNumber, startParkingRequest.ParkingName, startParkingRequest.Minutes);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("prepaid-stop")]
        public IActionResult StopParking(StopParkingRequest stopParkingRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.StopParking((int)stopParkingRequest.ParkingId, stopParkingRequest.IsAutoStopFlow, (int)stopParkingRequest.RemainingSeconds);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("around-areas")]
        public IActionResult GetParkingAroundAreas(AroundAreasRequest aroundAreasRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.GetParkingAroundAreas(aroundAreasRequest.Latitude, aroundAreasRequest.Longitude);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("generate-receipt")]
        public IActionResult GenerateReceipt(GenerateReceiptRequest generateReceiptRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.SendReceipt(generateReceiptRequest.MobileNumber, (int)generateReceiptRequest.ParkingId);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("history")]
        public IActionResult GetParkingHistory(ParkingHistoryRequest parkingHistoryRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.GetParkingHistory(parkingHistoryRequest.MobileNumber);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("save-note")]
        public IActionResult SaveParkingNote(SaveParkingNoteRequest saveParkingNoteRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.SaveParkingNote((int)saveParkingNoteRequest.ParkingId, saveParkingNoteRequest.ParkingNote);
            return Ok(basicResponse);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("get-active-parking-detail")]
        public IActionResult GetActiveParkingDetail(ParkingDetailsRequest parkingDetailsRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParking.GetParkingDetails(parkingDetailsRequest.RegistrationNumber);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult SearchParking(SearchParkingRequest searchParkingRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParkingLot.Get(searchParkingRequest.SearchString);
            return Ok(basicResponse);
        }
        #endregion
    }
}