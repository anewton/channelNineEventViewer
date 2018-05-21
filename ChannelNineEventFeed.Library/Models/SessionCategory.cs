using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Library.Models
{
    public class SessionCategory : DataEntity, ISessionCategory
    {
        public int SessionId { get; set; }

        public int CategoryId { get; set; }
    }
}
