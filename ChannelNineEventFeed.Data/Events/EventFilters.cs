using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Events
{
    public class EventFilters : IEventFilters
    {
        public EventFilters(List<string> eventNames = null, List<string> eventYears = null, List<string> levels = null, List<string> sessionTypes = null, List<int> categoryIds = null, List<int> speakerIds = null)
        {
            if (eventNames != null)
            {
                EventNames = eventNames;
            }

            if (eventYears != null)
            {
                EventYears = eventYears;
            }

            if (levels != null)
            {
                Levels = levels;
            }

            if (sessionTypes != null)
            {
                SessionTypes = sessionTypes;
            }

            if (categoryIds != null)
            {
                CategorIds = categoryIds;
            }

            if (speakerIds != null)
            {
                SpeakerIds = speakerIds;
            }
        }

        public List<string> EventNames { get; set; } = new List<string>();
        public List<string> EventYears { get; set; } = new List<string>();
        public List<string> Levels { get; set; } = new List<string>();
        public List<string> SessionTypes { get; set; } = new List<string>();
        public List<int> CategorIds { get; set; } = new List<int>();
        public List<int> SpeakerIds { get; set; } = new List<int>();
    }
}
