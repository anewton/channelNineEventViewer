using ChannelNineEventFeed.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelNineEventFeed.Library.Models
{
    public class Event : IEvent
    {
        public string Name { get; set; }

        public IEnumerable<string> Years { get; set; }
    }
}
