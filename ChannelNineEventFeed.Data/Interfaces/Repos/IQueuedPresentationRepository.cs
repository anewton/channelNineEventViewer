using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface IQueuedPresentationRepository : IRepository<IQueuedPresentation>
    {
        IQueuedPresentation FindByTitle(string title);
        IEnumerable<IQueuedPresentation> GetOrderedQueue();
    }
}
