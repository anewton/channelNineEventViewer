using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Library.Models
{
    public class Category : DataEntity, ICategory
    {
        public string Name { get; set; }
    }
}
