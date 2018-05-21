using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface ISessionRepository : IRepository<ISession>
    {
        int CountByEventNameAndYear(string eventName, string year);
        ISession FindByTitleAndEventYear(string title, string year);
        IEnumerable<int> FilterSessions(IEventFilters filters);
        IEnumerable<ISession> GetSessionsInSessionIdList(IEnumerable<int> sessionIdList);
        ISession FindById(int sessionId);
    }
}