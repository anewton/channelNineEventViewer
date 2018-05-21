using ChannelNineEventFeed.Library.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Models
{
    public class Session : DataEntity, ISession
    {
        [JsonProperty("id")]
        public string SessionId { get; set; }

        [JsonIgnore]
        public string EventYear { get; set; }

        [JsonIgnore]
        public string EventName { get; set; }

        [JsonProperty("starts")]
        public string Starts { get; set; }

        [JsonProperty("finish")]
        public string Finish { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("speakerids")]
        public IList<string> Speakerids { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("slides")]
        public string Slides { get; set; }

        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("track")]
        public object Track { get; set; }

        [JsonProperty("sessionType")]
        public string SessionType { get; set; }

        [JsonProperty("mediaduration")]
        public string Mediaduration { get; set; }

        [JsonProperty("iskeynote")]
        public bool Iskeynote { get; set; }

        [JsonProperty("prerecorded")]
        public bool Prerecorded { get; set; }

        [JsonProperty("notrecorded")]
        public bool Notrecorded { get; set; }

        [JsonProperty("previewimage")]
        public object Previewimage { get; set; }

        [JsonProperty("thumbnailimage")]
        public object Thumbnailimage { get; set; }

        [JsonProperty("video")]
        public string Video { get; set; }

        [JsonProperty("mp4med")]
        public string Mp4med { get; set; }

        [JsonProperty("mp4high")]
        public string Mp4high { get; set; }

        [JsonProperty("mp4low")]
        public string Mp4low { get; set; }

        [JsonProperty("wmvhq")]
        public string Wmvhq { get; set; }

        [JsonProperty("wmv")]
        public string Wmv { get; set; }

        [JsonProperty("smooth")]
        public string Smooth { get; set; }

        [JsonProperty("room")]
        public string Room { get; set; }
    }
}
