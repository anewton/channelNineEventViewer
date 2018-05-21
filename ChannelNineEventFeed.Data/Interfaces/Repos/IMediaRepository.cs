using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface IMediaRepository : IRepository<IMedia>
    {
        IMedia FindBySessionIdAndType(int sessionId, string mediaType);
        IEnumerable<IMedia> FindBySessionId(int sessionId);
        IMedia FindByMediaId(int mediaId);
    }
}