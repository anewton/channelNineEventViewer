using ChannelNineEventFeed.Library.Attributes;
using ChannelNineEventFeed.Library.Interfaces;
using Newtonsoft.Json;
using System;

namespace ChannelNineEventFeed.Library.Models
{
    public class DataEntity : IDataEntity
    {
        [PrimaryKey]
        [JsonIgnore]
        public int Id { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
