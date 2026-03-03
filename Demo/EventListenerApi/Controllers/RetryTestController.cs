using EventListenerApi.Brokers.Storages;
using EventListenerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventListenerApi.Controllers
{
    [ApiController]
    [Route("api/retry-test")]
    public class RetryTestController : ControllerBase
    {
        private static int requestCount = 0;

        private readonly IStorageBroker storageBroker;

        public RetryTestController(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        [HttpPost]
        public async ValueTask<ActionResult<Student>> Post([FromBody] Student student)
        {
            requestCount++;

            if (requestCount < 2)
            {
                return StatusCode(statusCode: 500, new
                {
                    Message = $"Simulated failure on request #{requestCount}"
                });
            }

            await this.storageBroker.InsertStudentAsync(student);

            return Ok(student);
        }
    }
}
