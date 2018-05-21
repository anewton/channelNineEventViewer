using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IFeedService
    {
        IEnumerable<IEvent> GetEvents();
        void DownloadFilteredEventData(IEventFilters filters);
    }
}