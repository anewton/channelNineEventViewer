using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Interfaces
{
    public interface ISession : IDataEntity
    {
        string SessionId { get; set; }
        string Code { get; set; }
        string Description { get; set; }
        string EventName { get; set; }
        string EventYear { get; set; }
        string Finish { get; set; }
        bool Iskeynote { get; set; }
        string Level { get; set; }
        string Link { get; set; }
        string Mediaduration { get; set; }
        string Mp4high { get; set; }
        string Mp4low { get; set; }
        string Mp4med { get; set; }
        bool Notrecorded { get; set; }
        bool Prerecorded { get; set; }
        string Previewimage { get; set; }
        string Room { get; set; }
        string SessionType { get; set; }
        string Slides { get; set; }
        string Smooth { get; set; }
        IList<string> Speakerids { get; set; }
        string Starts { get; set; }
        IList<string> Tags { get; set; }
        string Thumbnailimage { get; set; }
        string Title { get; set; }
        string Track { get; set; }
        string Video { get; set; }
        string Wmv { get; set; }
        string Wmvhq { get; set; }
    }
}