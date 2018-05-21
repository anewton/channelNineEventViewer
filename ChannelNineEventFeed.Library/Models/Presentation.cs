using ChannelNineEventFeed.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChannelNineEventFeed.Library.Models
{
    public class Presentation : IPresentation
    {
        public int Id { get; set; }

        public IEnumerable<ICategory> Categories { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Duration
        {
            get
            {
                if (Starts != null && Finish != null)
                {
                    var totalTime = Finish.Subtract(Starts).ToString();
                    return totalTime;
                }
                return string.Empty;
            }
        }

        public string EventName { get; set; }

        public string EventYear { get; set; }

        public DateTime Finish { get; set; }

        public string Level { get; set; }

        public string Link { get; set; }

        public IEnumerable<IMedia> Media { get; set; }

        public string SessionType { get; set; }

        public string SlidesLink { get; set; }

        public IEnumerable<ISpeaker> Speakers { get; set; }

        public DateTime Starts { get; set; }

        public string Thumbnailimage { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            var columns = typeof(Presentation).GetProperties().Select((x, i) => new PropertyMetaData() { Name = x.Name, PropertyType = x.PropertyType, Index = i }).ToArray();
            var presentationStringBuilder = new StringBuilder();
            foreach (var column in columns)
            {
                var propertyValue = this.GetType().GetProperty(column.Name).GetValue(this);
                presentationStringBuilder.AppendFormat("{0}=\"{1}\" ", column.Name, propertyValue == null ? string.Empty : propertyValue.ToString());
            }
            return presentationStringBuilder.ToString();
        }
    }
}
