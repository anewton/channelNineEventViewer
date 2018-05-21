namespace ChannelNineEventFeed.Data.Feeds
{
    public class SessionFeed
    {
        private const string BuildSessionBaseUrl = @"http://channel9.msdn.com/Events/{1}/{0}/sessions";
        private readonly string EventName = "Build";
        private readonly string Year = "2018";

        public SessionFeed(string eventName = "Build", string year = "2018")
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                EventName = eventName;
                Year = year;
            }
        }

        public string SessionFeedUrl
        {
            get
            {
                return string.Format(BuildSessionBaseUrl, Year, EventName);
            }
        }
    }
}
