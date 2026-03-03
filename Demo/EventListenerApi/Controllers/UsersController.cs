using EventListenerApi.Brokers.Storages;
using EventListenerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventListenerApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IStorageBroker storageBroker;

        public UsersController(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        [HttpPost]
        public async ValueTask<ActionResult<User>> Post([FromBody] User user)
        {
            await this.storageBroker.InsertUserAsync(user);

            return Ok(user);
        }
    }
}
