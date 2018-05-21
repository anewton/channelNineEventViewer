using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class VideoRepository : IVideoRepository
    {
        public VideoRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public IVideo Add(IVideo entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.Video.Add((Video)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(IVideo entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var video = context.Video.FirstOrDefault(x => x.Id == entity.Id);
                    if (video != null)
                    {
                        context.Entry(video).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public IVideo FindByTitle(string title)
        {
            IVideo result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Video.FirstOrDefault(x => x.Title == title);
                }
            }
            return result;
        }

        public IEnumerable<int> GetVideoIdListBySessionCode(string sessionCode)
        {
            IEnumerable<int> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Video.Where(x => x.SessionCode == sessionCode).Select(x => x.Id).ToList();
                }
            }
            return result;
        }

        public IEnumerable<int> GetVideoIdListBySessionList(IEnumerable<ISession> filteredSessions)
        {
            List<int> result = null;
            var events = filteredSessions.Select(x => new { Name = x.EventName, Year = x.EventYear }).Distinct().ToList();
            var sessionCodes = filteredSessions.Select(x => x.Code).ToList();
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    foreach (var sessionEvent in events)
                    {
                        var test = context.Video.Where(x => x.EventName == sessionEvent.Name && x.EventYear == sessionEvent.Year && sessionCodes.Contains(x.SessionCode)).Select(x => x.Id).Distinct().ToList();
                        if (result == null)
                        {
                            result = new List<int>();
                        }
                        result.AddRange(test);
                    }
                }
            }
            return result;
        }

        public IEnumerable<IVideo> GetVideosByVideoIdList(IEnumerable<int> videoIds)
        {
            IEnumerable<IVideo> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Video.Where(x => videoIds.Contains(x.Id)).ToList();
                }
            }
            return result;
        }
    }
}
