using System.Collections.Generic;
using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface ISpeakerRepository : IRepository<ISpeaker>
    {
        ISpeaker FindByNameAndEventYear(string eventName, string presenter, string year);
        IEnumerable<ISpeaker> GetWhereInSpeakerIdList(IEnumerable<int> speakerIds);
    }
}