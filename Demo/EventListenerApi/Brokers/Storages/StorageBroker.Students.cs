using EventListenerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventListenerApi.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Student> Students { get; set; }

        public async ValueTask<Student> InsertStudentAsync(Student student) =>
            await InsertAsync(student);
    }
}
