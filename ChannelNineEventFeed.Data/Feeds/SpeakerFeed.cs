using System;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Feeds
{
    public class SpeakerFeed
    {
        private const string AlphaSearchBaseUrl = @"http://channel9.msdn.com/Events/Speakers/firstLetter/";
        private const string EventSpeakersBaseUrl = @"https://channel9.msdn.com/Events/{2}/{0}/Speakers/rss?page={1}";
        private readonly string EventName = "Build";

        public SpeakerFeed(string eventName = "Build")
        {
            // Populate all firstLetter search rss feed urls
            AlphaSearchUrls = new List<string>();
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                AlphaSearchUrls.Add(AlphaSearchBaseUrl + letter.ToString().ToUpper());
            }
            if (!string.IsNullOrEmpty(eventName))
            {
                EventName = eventName;
            }
        }

        public List<string> AlphaSearchUrls { get; private set; }

        public string FeedYear { get; set; } = "2018";

        public string GetBuildSpeakerFeedUrlByPage(int page)
        {
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException("The page index cannot be less than or equal to zero");
            }
            return string.Format(EventSpeakersBaseUrl, FeedYear, page, EventName);
        }
    }
}
