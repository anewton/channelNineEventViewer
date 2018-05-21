using ChannelNineEventFeed.Library.Interfaces;
using System;

namespace ChannelNineEventFeed.Library.Models
{
    public class Media : DataEntity, IMedia
    {
        public int SessionId { get; set; }

        public string MediaType { get; set; }

        public bool? IsDownloaded { get; set; }

        public bool? IsDownloadInProgress { get; set; }

        public bool? IsPlayableInMediaElement { get; set; }

        public string DownloadLink { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
