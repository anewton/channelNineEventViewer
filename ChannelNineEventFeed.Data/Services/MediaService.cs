using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Data.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaService(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public void UpdateMedia(IMedia media)
        {
            _mediaRepository.Update(media);
        }

        public IMedia GetMediaById(int mediaId)
        {
            return _mediaRepository.FindByMediaId(mediaId);
        }
    }
}
