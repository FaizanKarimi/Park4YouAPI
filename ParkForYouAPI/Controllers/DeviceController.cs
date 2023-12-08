using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.Cache;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        #region Private Members
        private readonly IBODevice _boDevice;
        private readonly IOptions<AppSettings> _appSettings;
        #endregion

        #region Constructor
        public DeviceController(IBODevice boDevice, IOptions<AppSettings> appSettings)
        {
            _boDevice = boDevice;
            _appSettings = appSettings;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("add-device")]
        public IActionResult AddDevice(AddDeviceRequest addDeviceRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddDeviceRequest, Devices>();
            });
            IMapper mapper = config.CreateMapper();
            Devices devices = mapper.Map<Devices>(addDeviceRequest);
            _boDevice.Add(devices);
            return Ok(basicResponse);
        }
        #endregion
    }
}