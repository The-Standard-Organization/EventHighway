using EventListenerApi.Models;

namespace EventListenerApi.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
    }
}
