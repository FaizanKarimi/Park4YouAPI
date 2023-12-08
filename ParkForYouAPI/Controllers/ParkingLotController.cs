using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessOperations.Interfaces;
using Components.Identity;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.ParkingLot;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingLotController : ControllerBase
    {
        #region Private Members
        private readonly IBOParkingLot _bOParkingLot;
        private readonly UserManager<ApplicationUser> _userManager;        
        #endregion

        #region Constructor
        public ParkingLotController(IBOParkingLot bOParkingLot, UserManager<ApplicationUser> userManager)
        {
            _bOParkingLot = bOParkingLot;
            _userManager = userManager;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("get-parking-lots")]
        public async Task<IActionResult> GetParkingLotsAsync(ParkingLotRequest parkingLotRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await _userManager.FindByNameAsync(parkingLotRequest.Name);
            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                basicResponse.Data = _bOParkingLot.GetALL(user.Id, role.First());
            }
            return Ok(basicResponse);
        }

        [HttpGet]
        [Route("get-parking-lot/{id}")]
        public IActionResult GetParkingLot(int id)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParkingLot.Get(id);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("save-parking-lot")]
        public IActionResult UpdateParkingLot(UpdateParkingLotRequest saveParkingLotRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateParkingLotRequest, ParkingLots>();
            });
            IMapper mapper = config.CreateMapper();
            ParkingLots parkingLot = mapper.Map<ParkingLots>(saveParkingLotRequest);
            basicResponse.Data = _bOParkingLot.Update(parkingLot);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("delete-parking-lot")]
        public IActionResult DeleteParkingLot(DeleteParkingLotRequest deleteParkingLotRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOParkingLot.Delete((int)deleteParkingLotRequest.Id);
            return Ok(basicResponse);
        }
        #endregion
    }
}