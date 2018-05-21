namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IPresentationMetaData
    {
        int SessionId { get; set; }
        int CategoryId { get; set; }
        string EventName { get; set; }
        string EventYear { get; set; }
        string Level { get; set; }
        string SessionType { get; set; }
    }
}