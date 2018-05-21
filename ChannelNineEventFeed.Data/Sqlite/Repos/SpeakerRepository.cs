using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class SpeakerRepository : ISpeakerRepository
    {
        public SpeakerRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public ISpeaker Add(ISpeaker entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.Speaker.Add((Speaker)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(ISpeaker entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var session = context.Speaker.FirstOrDefault(x => x.Id == entity.Id);
                    if (session != null)
                    {
                        context.Entry(session).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public ISpeaker FindByNameAndEventYear(string eventName, string presenter, string year)
        {
            ISpeaker result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Speaker.FirstOrDefault(x => x.EventName == eventName && x.Name == presenter && x.EventYear == year);
                }
            }
            return result;
        }

        public IEnumerable<ISpeaker> GetWhereInSpeakerIdList(IEnumerable<int> speakerIds)
        {
            IEnumerable<ISpeaker> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Speaker.Where(x => speakerIds.Contains(x.Id)).ToList();
                }
            }
            return result;
        }
    }
}
