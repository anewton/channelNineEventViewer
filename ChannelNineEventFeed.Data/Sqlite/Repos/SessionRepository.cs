using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class SessionRepository : ISessionRepository
    {
        public SessionRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public ISession Add(ISession entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.Session.Add((Session)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(ISession entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var session = context.Session.FirstOrDefault(x => x.SessionId == entity.SessionId);
                    if (session != null)
                    {
                        context.Entry(session).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public int CountByEventNameAndYear(string eventName, string year)
        {
            var result = 0;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Session.Count(x => x.EventName == eventName && x.EventYear == year);
                }
            }
            return result;
        }

        public ISession FindByTitleAndEventYear(string title, string year)
        {
            ISession result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Session.FirstOrDefault(x => x.Title == title && x.EventYear == year);
                }
            }
            return result;
        }

        public IEnumerable<int> FilterSessions(IEventFilters filters)
        {
            IEnumerable<int> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var query = context.Session.SelectMany(
                        session => context.SessionCategory.Where(sessionCategory => session.Id == sessionCategory.SessionId).DefaultIfEmpty(),
                        (session, sessionCategory) => new PresentationMetaData
                        {
                            SessionId = session.Id,
                            CategoryId = sessionCategory.CategoryId,
                            //SpeakerIds = session.Speakerids,
                            EventName = session.EventName,
                            EventYear = session.EventYear,
                            Level = session.Level,
                            SessionType = session.SessionType
                        });
                    if (filters.EventNames != null && filters.EventNames.Count > 0)
                    {
                        query = query.Where<PresentationMetaData>(p => filters.EventNames.Contains(p.EventName));
                    }
                    if (filters.EventYears != null && filters.EventYears.Count > 0)
                    {
                        query = query.Where<PresentationMetaData>(p => filters.EventYears.Contains(p.EventYear));
                    }
                    if (filters.Levels != null && filters.Levels.Count > 0)
                    {
                        query = query.Where<PresentationMetaData>(p => filters.Levels.Contains(p.Level));
                    }
                    if (filters.SessionTypes != null && filters.SessionTypes.Count > 0)
                    {
                        query = query.Where<PresentationMetaData>(p => filters.SessionTypes.Contains(p.SessionType));
                    }
                    if (filters.CategoryIds != null && filters.CategoryIds.Count > 0)
                    {
                        query = query.Where<PresentationMetaData>(p => filters.CategoryIds.Contains(p.CategoryId));
                    }
                    //if (filters.SpeakerIds != null && filters.SpeakerIds.Count > 0)
                    //{
                    //    query = query.Where<PresentationMetaData>(p => filters.SpeakerIds.Contains(p.SpeakerId));
                    //}
                    result = query.Select(p => p.SessionId).Distinct().ToList();
                }
            }
            return result;
        }

        public IEnumerable<ISession> GetSessionsInSessionIdList(IEnumerable<int> sessionIdList)
        {
            IEnumerable<ISession> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Session.Where(x => sessionIdList.Contains(x.Id)).ToList();
                }
            }
            return result;
        }

        public ISession FindById(int sessionId)
        {
            ISession result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Session.FirstOrDefault(x => x.Id == sessionId);
                }
            }
            return result;
        }
    }
}