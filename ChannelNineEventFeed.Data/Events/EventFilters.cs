using ChannelNineEventFeed.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChannelNineEventFeed.Data.Events
{
    public class EventFilters : IEventFilters
    {
        public EventFilters(List<string> eventNames = null, List<string> eventYears = null, List<string> levels = null, List<string> sessionTypes = null, List<int> categoryIds = null, List<string> categoryNames = null, List<int> speakerIds = null, List<string> speakerNames = null)
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
                CategoryIds = categoryIds;
            }

            if (categoryNames != null)
            {
                CategoryNames = categoryNames;
            }

            if (speakerIds != null)
            {
                SpeakerIds = speakerIds;
            }

            if (speakerNames != null)
            {
                SpeakerNames = speakerNames;
            }
        }

        public List<string> EventNames { get; set; } = new List<string>();
        public List<string> EventYears { get; set; } = new List<string>();
        public List<string> Levels { get; set; } = new List<string>();
        public List<string> SessionTypes { get; set; } = new List<string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<int> SpeakerIds { get; set; } = new List<int>();
        public List<string> SpeakerNames { get; set; } = new List<string>();

        public override string ToString()
        {
            var filterSummary = new StringBuilder();
            filterSummary.Append(CreateFilterSummaryText(EventNames));
            filterSummary.Append(CreateFilterSummaryText(EventYears, true));
            filterSummary.Append(CreateFilterSummaryText(Levels, true));
            filterSummary.Append(CreateFilterSummaryText(SessionTypes, true));
            filterSummary.Append(CreateFilterSummaryText(CategoryNames, true));
            filterSummary.Append(CreateFilterSummaryText(SpeakerNames, true));
            return filterSummary.ToString();
        }

        public string GetXamlToString()
        {
            var filterSummary = new StringBuilder();
            filterSummary.Append(CreateFilterSummaryText(EventNames, includeXamlFormatting: true));
            filterSummary.Append(CreateFilterSummaryText(EventYears, true, true));
            filterSummary.Append(CreateFilterSummaryText(Levels, true, true));
            filterSummary.Append(CreateFilterSummaryText(SessionTypes, true, true));
            filterSummary.Append(CreateFilterSummaryText(CategoryNames, true, true));
            filterSummary.Append(CreateFilterSummaryText(SpeakerNames, true, true));
            return filterSummary.ToString();
        }

        public bool HasAtLeastOneFilter()
        {
            // Must at least have one EventName selected
            if (EventNames != null && EventNames.Count > 0)
            {
                return true;
            }
            else if (EventNames.Count == 0)
            {
                return false;
            }
            if (EventYears != null && EventYears.Count > 0)
            {
                return true;
            }
            if (Levels != null && Levels.Count > 0)
            {
                return true;
            }
            if (SessionTypes != null && SessionTypes.Count > 0)
            {
                return true;
            }
            if (CategoryIds != null && CategoryIds.Count > 0)
            {
                return true;
            }
            if (SpeakerIds != null && SpeakerIds.Count > 0)
            {
                return true;
            }
            return false;
        }

        private string CreateFilterSummaryText<T>(List<T> filterData, bool includeAndPrefix = false, bool includeXamlFormatting = false)
        {
            var filterSummary = new StringBuilder();
            const string And = "AND";
            const string Or = "OR";
            const string OpenParen = "(";
            const string CloseParen = ")";
            const string Space = " ";
            if (filterData != null && filterData.Count > 0)
            {
                if (includeAndPrefix)
                {
                    filterSummary.Append(Space);
                    filterSummary.Append(And);
                    filterSummary.Append(Space);
                }
                var isGrouped = filterData.Count > 1;
                if (isGrouped)
                {
                    filterSummary.Append(OpenParen);
                }
                foreach (var item in filterData)
                {
                    if (item != null)
                    {
                        filterSummary.Append(Space);
                        if (includeXamlFormatting)
                        {
                            filterSummary.Append(string.Format("<Bold>{0}</Bold>", item.ToString()));
                        }
                        else
                        {
                            filterSummary.Append(item.ToString());
                        }
                        filterSummary.Append(Space);
                        if (isGrouped && filterData.IndexOf(item) != filterData.Count - 1)
                        {
                            filterSummary.Append(Or);
                        }
                    }
                }
                if (isGrouped)
                {
                    filterSummary.Append(CloseParen);
                }
            }
            return filterSummary.ToString();
        }
    }
}
