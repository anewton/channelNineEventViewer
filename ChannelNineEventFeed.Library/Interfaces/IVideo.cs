using System;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IVideo : IDataEntity
    {
        int DurationSeconds { get; set; }
        string EventName { get; set; }
        string EventYear { get; set; }
        string HostedPageLink { get; set; }
        bool? IsVideoViewingComplete { get; set; }
        double? MyRatingScore { get; set; }
        DateTime? PublishDate { get; set; }
        string SessionCode { get; set; }
        string Summary { get; set; }
        string SummaryHtml { get; set; }
        string Title { get; set; }
    }
}