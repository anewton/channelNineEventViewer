using System;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface IPresentation
    {
        int Id { get; set; }
        IEnumerable<ICategory> Categories { get; set; }
        string Code { get; set; }
        string Description { get; set; }
        string Duration { get; }
        string EventName { get; set; }
        string EventYear { get; set; }
        DateTime Finish { get; set; }
        string Level { get; set; }
        string Link { get; set; }
        IEnumerable<IMedia> Media { get; set; }
        string SessionType { get; set; }
        string SlidesLink { get; set; }
        IEnumerable<ISpeaker> Speakers { get; set; }
        DateTime Starts { get; set; }
        string Thumbnailimage { get; set; }
        string Title { get; set; }
        string ToString();
    }
}