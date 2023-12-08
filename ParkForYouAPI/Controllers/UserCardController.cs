using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels;
using ParkForYouAPI.APIRequestModels.UserCards;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserCardController : ControllerBase
    {
        #region Private Members
        private readonly IBOUserCard _bOUserCard;
        #endregion

        #region Constructor
        public UserCardController(IBOUserCard bOUserCard)
        {
            _bOUserCard = bOUserCard;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("get-all")]
        public IActionResult Get(GetUserCardsRequest getUserCardsRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserCard.GetALL(getUserCardsRequest.MobileNumber);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult Delete(DeleteUserCardRequest deleteUserCardRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserCard.Delete(deleteUserCardRequest.MobileNumber, (int)deleteUserCardRequest.CardId);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(AddUserCardRequest addUserCardRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddUserCardRequest, UserCards>();
            });
            IMapper mapper = config.CreateMapper();
            UserCards userCards = mapper.Map<UserCards>(addUserCardRequest);
            basicResponse.Data = _bOUserCard.Add(userCards).Id;
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("set-default-card")]
        public IActionResult SetDefaultUserCard(DefaultUserCardRequest defaultUserCardRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = _bOUserCard.MarkedUserCardDefault((int)defaultUserCardRequest.CardId, defaultUserCardRequest.MobileNumber);
            return Ok(basicResponse);
        }
        #endregion
    }
}