using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.UserSetting;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserSettingController : ControllerBase
    {
        #region Private Members
        private readonly IBOUserSetting _bOUserSetting;
        #endregion

        #region Constructor
        public UserSettingController(IBOUserSetting bOUserSetting)
        {
            _bOUserSetting = bOUserSetting;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("update-settings")]
        public IActionResult UpdateSettings(UpdateUserSettingRequest updateUserSettingRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateUserSettingRequest, UserSettings>();
            });
            IMapper mapper = config.CreateMapper();
            UserSettings userSettings = mapper.Map<UserSettings>(updateUserSettingRequest);
            basicResponse.Data = _bOUserSetting.Update(userSettings);
            return Ok(basicResponse);
        }
        #endregion
    }
}