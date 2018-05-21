using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface ISessionCategoryRepository : IRepository<ISessionCategory>
    {
        ISessionCategory FindBySessionIdAndCategoryId(int sessionId, int id);
        IEnumerable<ISessionCategory> FindBySessionId(IEnumerable<int> sessionIdList);
    }
}