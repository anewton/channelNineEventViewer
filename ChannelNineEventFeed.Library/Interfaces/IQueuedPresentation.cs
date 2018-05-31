namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IQueuedPresentation : IDataEntity
    {
        int OrderIndex { get; set; }
        string Title { get; set; }
    }
}
