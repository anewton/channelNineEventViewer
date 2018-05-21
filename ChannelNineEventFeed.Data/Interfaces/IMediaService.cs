using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IMediaService
    {
        void UpdateMedia(IMedia media);
        IMedia GetMediaById(int mediaId);
    }
}
