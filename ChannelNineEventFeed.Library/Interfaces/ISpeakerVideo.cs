namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface ISpeakerVideo : IDataEntity
    {
        int SpeakerId { get; set; }
        int VideoId { get; set; }
    }
}