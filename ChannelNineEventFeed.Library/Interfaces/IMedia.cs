using System;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IMedia : IDataEntity
    {
        string DownloadLink { get; set; }
        bool? IsDownloaded { get; set; }
        bool? IsDownloadInProgress { get; set; }
        bool? IsPlayableInMediaElement { get; set; }
        string MediaType { get; set; }
        DateTime? PublishDate { get; set; }
        int SessionId { get; set; }
    }
}