using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Intefaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ChannelNineEventFeed.Data.Services
{
    public class EventService : IEventService
    {
        private readonly IAppSettings _appSettings;

        public EventService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IEnumerable<IEvent> GetEvents()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ChannelNineEventFeed.Data.Events.eventList.json";
            var eventListJson = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    eventListJson = reader.ReadToEnd();
                }
            }
            if (!string.IsNullOrEmpty(eventListJson))
            {
                return JsonConvert.DeserializeObject<List<Event>>(eventListJson);
            }
            return null;
        }
    }
}
