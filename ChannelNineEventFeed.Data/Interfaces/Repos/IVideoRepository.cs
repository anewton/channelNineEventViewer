using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface IVideoRepository : IRepository<IVideo>
    {
        IVideo FindByTitle(string title);
        IEnumerable<int> GetVideoIdListBySessionCode(string sessionCode);
        IEnumerable<int> GetVideoIdListBySessionList(IEnumerable<ISession> filteredSessions);
        IEnumerable<IVideo> GetVideosByVideoIdList(IEnumerable<int> videoIds);
    }
}