using EventListenerApi.Brokers.Storages;
using EventListenerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventListenerApi.Controllers
{
    [ApiController]
    [Route("api/schools")]
    public class SchoolsController : ControllerBase
    {
        private readonly IStorageBroker storageBroker;

        public SchoolsController(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        [HttpPost]
        public async ValueTask<ActionResult<School>> Post([FromBody] School school)
        {
            await this.storageBroker.InsertSchoolAsync(school);

            return Ok(school);
        }
    }
}
