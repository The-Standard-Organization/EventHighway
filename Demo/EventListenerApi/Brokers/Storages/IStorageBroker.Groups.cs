using EventListenerApi.Models;

namespace EventListenerApi.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Group> InsertGroupAsync(Group group);
    }
}
