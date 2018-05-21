using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class MediaRepository : IMediaRepository
    {
        public MediaRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public IMedia Add(IMedia entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.Media.Add((Media)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(IMedia entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var media = context.Media.FirstOrDefault(x => x.Id == entity.Id);
                    if (media != null)
                    {
                        context.Entry(media).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public IMedia FindBySessionIdAndType(int sessionId, string mediaType)
        {
            IMedia result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Media.FirstOrDefault(x => x.SessionId == sessionId && x.MediaType == mediaType);
                }
            }
            return result;
        }

        public IEnumerable<IMedia> FindBySessionId(int sessionId)
        {
            IEnumerable<IMedia> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Media.Where(x => x.SessionId == sessionId).ToList();
                }
            }
            return result;
        }

        public IMedia FindByMediaId(int mediaId)
        {
            IMedia result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Media.FirstOrDefault(x => x.Id == mediaId);
                }
            }
            return result;
        }
    }
}