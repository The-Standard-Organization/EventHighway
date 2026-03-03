using EventListenerApi.Brokers.Storages;
using EventListenerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventListenerApi.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IStorageBroker storageBroker;

        public GroupsController(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        [HttpPost]
        public async ValueTask<ActionResult<Group>> Post([FromBody] Group group)
        {
            await this.storageBroker.InsertGroupAsync(group);

            return Ok(group);
        }
    }
}
