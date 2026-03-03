using EventListenerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventListenerApi.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Group> Groups { get; set; }

        public async ValueTask<Group> InsertGroupAsync(Group group) =>
            await InsertAsync(group);
    }
}
