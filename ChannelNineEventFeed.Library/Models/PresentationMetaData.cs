using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Models
{
    public class PresentationMetaData : IPresentationMetaData
    {
        public int SessionId { get; set; }

        public int CategoryId { get; set; }

        //public IEnumerable<int> SpeakerIds { get; set; }

        public string EventName { get; set; }

        public string EventYear { get; set; }

        public string Level { get; set; }

        public string SessionType { get; set; }
    }
}
