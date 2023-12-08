using BusinessOperations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.Price;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        #region Private Members
        private readonly IBOParkingLot _bOParkingLot;
        #endregion

        #region Constructor
        public PriceController(IBOParkingLot bOParkingLot)
        {
            _bOParkingLot = bOParkingLot;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("check-parking-price")]
        public IActionResult CheckPrice(PriceCheckRequest priceCheckRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParkingLot.GetParkingPrice((int)priceCheckRequest.ParkingLotId, (int)priceCheckRequest.Minutes);
            return Ok(basicResponse);
        }
        #endregion
    }
}