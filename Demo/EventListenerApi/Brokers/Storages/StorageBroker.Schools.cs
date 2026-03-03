using EventListenerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventListenerApi.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<School> Schools { get; set; }

        public async ValueTask<School> InsertSchoolAsync(School school) =>
            await InsertAsync(school);
    }
}
