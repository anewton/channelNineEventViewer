using ChannelNineEventFeed.Library.Interfaces;
using System;

namespace ChannelNineEventFeed.Library.Models
{
    public class Speaker : DataEntity, ISpeaker
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string HostedPageLink { get; set; }

        public string EventYear { get; set; }

        public string EventName { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
