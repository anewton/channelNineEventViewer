using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface ISpeakerVideoRepository : IRepository<ISpeakerVideo>
    {
        ISpeakerVideo FindBySpeakerIdAndVideoId(int speakerId, int videoId);
        IEnumerable<int> GetSpeakerIdListWhereInVideoIdList(IEnumerable<int> videoIds);
        IEnumerable<int> GetVideoIdListWhereInSpeakerIdList(IEnumerable<int> speakerIds);
    }
}