using System.Collections.Generic;
using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.ChargeSheet;
using ParkForYouAPI.APIRequestModels.ChargeSheetPrices;
using ParkForYouAPI.APIRequestModels.ParkingLot;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeSheetController : ControllerBase
    {
        #region Private Members
        private readonly IBOParkingLot _bOParkingLot;
        private readonly IBOChargeSheet _bOChargeSheet;
        #endregion

        #region Constructor
        public ChargeSheetController(IBOParkingLot bOParkingLot, IBOChargeSheet bOChargeSheet)
        {
            _bOParkingLot = bOParkingLot;
            _bOChargeSheet = bOChargeSheet;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("update-charge-sheet")]
        public IActionResult UpdateChargeSheet(UpdateChargeSheetRequest updateChargeSheetRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateChargeSheetRequest, ChargeSheets>();
                cfg.CreateMap<UpdateChargeSheetPricesRequest, ChargeSheetPrices>();
            });
            IMapper mapper = config.CreateMapper();
            ChargeSheets chargeSheet = mapper.Map<ChargeSheets>(updateChargeSheetRequest);
            chargeSheet.ChargeSheetPrices = mapper.Map<List<ChargeSheetPrices>>(updateChargeSheetRequest.updateChargeSheetPricesRequest);
            basicResponse.Data = _bOChargeSheet.Update(chargeSheet, chargeSheet.ChargeSheetPrices);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("add-charge-sheet")]
        public IActionResult AddChargeSheet(AddParkingLotRequest addParkingLotRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddParkingLotRequest, ParkingLots>();
                cfg.CreateMap<SaveChargeSheetRequest, ChargeSheets>();
                cfg.CreateMap<SaveChargeSheetPricesRequest, ChargeSheetPrices>();
            });
            IMapper mapper = config.CreateMapper();
            ParkingLots parkingLot = mapper.Map<ParkingLots>(addParkingLotRequest);
            parkingLot.ChargeSheet = mapper.Map<ChargeSheets>(addParkingLotRequest.saveChargeSheetRequest);
            parkingLot.ChargeSheetPrices = mapper.Map<List<ChargeSheetPrices>>(addParkingLotRequest.saveChargeSheetPricesRequests);
            basicResponse.Data = _bOParkingLot.Save(parkingLot, parkingLot.ChargeSheet, parkingLot.ChargeSheetPrices);
            return Ok(basicResponse);
        }

        [HttpGet]
        [Route("get-charge-sheet/{id}")]
        public IActionResult GetChargeSheet(int id)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOChargeSheet.Get(id);
            return Ok(basicResponse);
        }
        #endregion
    }
}