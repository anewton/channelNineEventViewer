using ChannelNineEventFeed.Library.Interfaces;
using System;

namespace ChannelNineEventFeed.Library.Models
{
    public class Video : DataEntity, IVideo
    {
        public string Title { get; set; }

        public string SummaryHtml { get; set; }

        public string Summary { get; set; }

        public string EventYear { get; set; }

        public string EventName { get; set; }

        public int DurationSeconds { get; set; }

        public DateTime? PublishDate { get; set; }

        public string HostedPageLink { get; set; }

        public bool? IsVideoViewingComplete { get; set; }

        public double? MyRatingScore { get; set; }

        public string SessionCode { get; set; }
    }
}
