using System.Collections.Generic;

namespace ChannelNineEventFeed.WPF
{
    public static class CollectionExtensions
    {
        public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> addSource)
        {
            foreach (T item in addSource)
            {
                source.Add(item);
            }
            return source;
        }
    }
}