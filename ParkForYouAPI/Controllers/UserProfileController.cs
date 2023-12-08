using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.UserProfile;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        #region Private Members
        private readonly IBOUserProfile _bOUserProfile;        
        #endregion

        #region Constructor
        public UserProfileController(IBOUserProfile bOUserProfile)
        {
            _bOUserProfile = bOUserProfile;            
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("update-profile")]
        public IActionResult UpdateUserProfile(UpdateUserProfileRequest updateUserProfileRequest)
        {            
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateUserProfileRequest, UserProfiles>();
            });
            IMapper mapper = config.CreateMapper();
            UserProfiles userProfiles = mapper.Map<UserProfiles>(updateUserProfileRequest);
            basicResponse.Data = _bOUserProfile.Update(userProfiles);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("get-profile")]
        public IActionResult GetProfile(GetUserProfileRequest getUserProfileRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserProfile.GetUserProfileInformation(getUserProfileRequest.MobileNumber);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("send-data-to-email")]
        public IActionResult SendDataToEmail(SendDataToEmailRequest sendDataToEmailRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            _bOUserProfile.SendDataToEmail(sendDataToEmailRequest.MobileNumber);
            return Ok(basicResponse);
        }
        #endregion
    }
}