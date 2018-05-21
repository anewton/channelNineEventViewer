using System;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Feeds
{
    public sealed class VideoFeed
    {
        public static readonly VideoFeed WMVHigh = new VideoFeed("wmvhigh", "Wmvhq", "video/x-m-wmv", ".wmv");
        public static readonly VideoFeed WMV = new VideoFeed("wmv", "Wmv", "video/x-m-wmv", ".wmv");
        public static readonly VideoFeed MP4High = new VideoFeed("mp4high", "Mp4high", "video/mp4", ".mp4");
        public static readonly VideoFeed Mp4Med = new VideoFeed("mp4med", "Mp4med", "video/mp4", ".mp4");
        public static readonly VideoFeed Mp4Low = new VideoFeed("mp4", "Mp4low", "video/mp4", ".mp4");

        private const string BaseRSSUrl = @"http://s.ch9.ms/Events/{3}/{0}/RSS/{1}?page={2}";
        private readonly string SessionMediaType;

        private VideoFeed(string type, string sessionMediaType, string mediaType, string mediaFileExtension)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Parameter type is required");
            }
            if (string.IsNullOrEmpty(sessionMediaType))
            {
                throw new ArgumentException("Parameter sessionMediaType is required");
            }
            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentException("Parameter mediaType is required");
            }
            if (string.IsNullOrEmpty(mediaFileExtension))
            {
                throw new ArgumentException("Parameter mediaFileExtension is required");
            }
            Type = type;
            SessionMediaType = sessionMediaType;
            MediaType = mediaType;
            MediaFileExtension = mediaFileExtension;
            if (VideoFeeds == null)
            {
                VideoFeeds = new List<VideoFeed>();
            }
            VideoFeeds.Add(this);
        }

        public static List<VideoFeed> VideoFeeds { get; private set; }

        public string MediaType { get; private set; }

        public string MediaFileExtension { get; private set; }

        public string Type { get; private set; }

        public string FeedYear { get; set; } = "2018";

        public string GetBuildVideoFeedUrlByPage(int page, string eventName = "Build")
        {
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException("The page index cannot be less than or equal to zero");
            }
            eventName = string.IsNullOrEmpty(eventName) ? "Build" : eventName;
            return string.Format(BaseRSSUrl, FeedYear, Type, page, eventName);
        }
    }
}
