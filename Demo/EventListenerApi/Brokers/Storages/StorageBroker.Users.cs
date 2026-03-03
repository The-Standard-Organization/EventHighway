using EventListenerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventListenerApi.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);
    }
}
