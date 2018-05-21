using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class SessionCategoryRepository : ISessionCategoryRepository
    {
        public SessionCategoryRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public ISessionCategory Add(ISessionCategory entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.SessionCategory.Add((SessionCategory)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(ISessionCategory entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var sessionCategory = context.SessionCategory.FirstOrDefault(x => x.Id == entity.Id);
                    if (sessionCategory != null)
                    {
                        context.Entry(sessionCategory).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public ISessionCategory FindBySessionIdAndCategoryId(int sessionId, int categoryId)
        {
            ISessionCategory result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.SessionCategory.FirstOrDefault(x => x.SessionId == sessionId && x.CategoryId == categoryId);
                }
            }
            return result;
        }

        public IEnumerable<ISessionCategory> FindBySessionId(IEnumerable<int> sessionIdList)
        {
            IEnumerable<ISessionCategory> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.SessionCategory.Where(x => sessionIdList.Contains(x.SessionId)).ToList();
                }
            }
            return result;
        }
    }
}
