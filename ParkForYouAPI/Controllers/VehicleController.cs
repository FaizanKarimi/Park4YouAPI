using System.Collections.Generic;
using AutoMapper;
using BusinessOperations.Interfaces;
using Infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels;
using ParkForYouAPI.APIRequestModels.Vehicles;

namespace ParkForYouAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        #region Private Members
        private readonly IBOVehicles _bOVehicles;
        #endregion

        #region Constructor
        public VehicleController(IBOVehicles bOVehicles)
        {
            _bOVehicles = bOVehicles;
        }
        #endregion

        #region API'S
        [HttpPost]
        [Route("add")]
        public IActionResult Add(AddVehicleRequest addVehicleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddVehicleRequest, Vehicles>();
            });
            IMapper mapper = config.CreateMapper();
            Vehicles vehicle = mapper.Map<Vehicles>(addVehicleRequest);
            Vehicles addedVehicle = _bOVehicles.Add(vehicle);
            basicResponse.Data = addedVehicle.Id;
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("get-all")]
        public IActionResult GetALL(GetVehicleRequest getVehicleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            List<Vehicles> vehicles = _bOVehicles.GetALL(getVehicleRequest.MobileNumber);
            basicResponse.Data = vehicles;
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult Edit(UpdateVehicleRequest updateVehicleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateVehicleRequest, Vehicles>();
            });
            IMapper mapper = config.CreateMapper();
            Vehicles vehicle = mapper.Map<Vehicles>(updateVehicleRequest);
            bool IsUpdated = _bOVehicles.Update(vehicle);
            basicResponse.Data = true;
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult Delete(DeleteVehicleRequest deleteVehicleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DeleteVehicleRequest, Vehicles>()
                .ForPath(destination => destination.Id, option => option.MapFrom(source => (int)source.VehicleId));
            });
            IMapper mapper = config.CreateMapper();
            Vehicles vehicle = mapper.Map<Vehicles>(deleteVehicleRequest);
            bool IsDeleted = _bOVehicles.Delete(vehicle);
            basicResponse.Data = IsDeleted;
            return Ok(basicResponse);
        }
        #endregion
    }
}