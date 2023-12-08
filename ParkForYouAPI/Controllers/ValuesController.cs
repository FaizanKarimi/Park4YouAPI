using Microsoft.AspNetCore.Mvc;
using ParkForYouAPI.APIModels;

namespace ParkForYouAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            BasicResponse basicResponse = new BasicResponse();            
            basicResponse.Data = new string[] { "value1", "value2" };
            return Ok(basicResponse);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            BasicResponse basicResponse = new BasicResponse();
            basicResponse.Data = "value";
            return Ok(basicResponse);
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}