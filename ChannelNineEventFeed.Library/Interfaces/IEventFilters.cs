using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IEventFilters
    {
        List<int> CategoryIds { get; set; }
        List<string> EventNames { get; set; }
        List<string> EventYears { get; set; }
        List<string> Levels { get; set; }
        List<string> SessionTypes { get; set; }
        List<int> SpeakerIds { get; set; }
    }
}