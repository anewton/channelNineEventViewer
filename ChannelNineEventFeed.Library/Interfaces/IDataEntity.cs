using System;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IDataEntity
    {
        int Id { get; set; }

        DateTime? Created { get; set; }

        DateTime? LastUpdated { get; set; }
    }
}
