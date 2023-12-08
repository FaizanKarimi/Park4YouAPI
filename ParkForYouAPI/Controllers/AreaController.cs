using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.Cache;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.Area;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        #region Private Members
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBOUserArea _bOUserArea;
        #endregion

        #region Constructor
        public AreaController(IOptions<AppSettings> appSettings, IBOUserArea bOUserArea)
        {
            _appSettings = appSettings;
            _bOUserArea = bOUserArea;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("delete")]
        public IActionResult Delete(DeleteAreaRequest deleteAreaRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserArea.Delete((int)deleteAreaRequest.Id);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(AddAreaRequest addAreaRequest)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddAreaRequest, UserAreas>();
            });
            IMapper mapper = config.CreateMapper();
            UserAreas userAreas = mapper.Map<UserAreas>(addAreaRequest);
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserArea.Add(userAreas).Id;
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("get-all-area")]
        public IActionResult Get(GetAreaRequest getAreaRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserArea.GetUserAreas(getAreaRequest.MobileNumber);
            return Ok(basicResponse);
        }
        #endregion
    }
}