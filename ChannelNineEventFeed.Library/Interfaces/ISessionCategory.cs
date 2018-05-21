namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface ISessionCategory : IDataEntity
    {
        int CategoryId { get; set; }
        int SessionId { get; set; }
    }
}