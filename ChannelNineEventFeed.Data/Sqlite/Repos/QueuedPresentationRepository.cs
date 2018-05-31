using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class QueuedPresentationRepository : IQueuedPresentationRepository
    {
        public QueuedPresentationRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public IQueuedPresentation Add(IQueuedPresentation entity)
        {
            var existing = FindByTitle(entity.Title);
            if (existing != null)
            {
                return existing;
            }
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.QueuedPresentation.Add((QueuedPresentation)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(IQueuedPresentation entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var queuedPresentation = context.QueuedPresentation.FirstOrDefault(x => x.Id == entity.Id);
                    if (queuedPresentation != null)
                    {
                        context.Entry(queuedPresentation).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public IQueuedPresentation FindByTitle(string title)
        {
            IQueuedPresentation result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.QueuedPresentation.FirstOrDefault(x => x.Title.ToLower() == title.ToLower());
                }
            }
            return result;
        }

        public IEnumerable<IQueuedPresentation> GetOrderedQueue()
        {
            IEnumerable<IQueuedPresentation> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.QueuedPresentation.OrderBy(x => x.OrderIndex).ToList();
                }
            }
            return result;
        }
    }
}
