using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Library.Models
{
    public class SpeakerVideo : DataEntity, ISpeakerVideo
    {
        public int SpeakerId { get; set; }

        public int VideoId { get; set; }
    }
}
