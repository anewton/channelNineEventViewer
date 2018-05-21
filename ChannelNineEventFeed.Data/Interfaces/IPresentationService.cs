using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IPresentationService
    {
        IEnumerable<IPresentation> GetFilteredPresentations(IEventFilters filters);
        IPresentation SessionToPresentation(ISession session);
        IPresentation GetPresentationBySessionId(int sessionId);
        ISession GetSessionBySessionId(int sessionId);
        IEnumerable<ICategory> GetFilteredCategories(IEventFilters filters);
        IEnumerable<string> GetFilteredSessionTypes(IEventFilters filters);
        IEnumerable<ISpeaker> GetFilteredSpeakers(IEventFilters filters);
    }
}
