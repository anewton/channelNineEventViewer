using System;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface ISpeaker : IDataEntity
    {
        string EventName { get; set; }
        string EventYear { get; set; }
        string HostedPageLink { get; set; }
        string Name { get; set; }
        DateTime? PublishDate { get; set; }
        string Summary { get; set; }
    }
}