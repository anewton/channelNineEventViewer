using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IEvent
    {
        string Name { get; set; }
        IEnumerable<string> Years { get; set; }
    }
}
