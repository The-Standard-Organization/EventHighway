using EventListenerApi.Brokers.Storages;
using EventListenerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventListenerApi.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStorageBroker storageBroker;

        public StudentsController(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        [HttpPost]
        public async ValueTask<ActionResult<Student>> Post([FromBody] Student student)
        {
            await this.storageBroker.InsertStudentAsync(student);

            return Ok(student);
        }
    }
}
