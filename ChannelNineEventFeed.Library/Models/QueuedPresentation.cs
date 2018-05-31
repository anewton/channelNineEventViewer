using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Library.Models
{
    public class QueuedPresentation : DataEntity, IQueuedPresentation
    {
        public int OrderIndex { get; set; }

        public string Title { get; set; }
    }
}