using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IEventService
    {
        IEnumerable<IEvent> GetEvents();
    }
}
