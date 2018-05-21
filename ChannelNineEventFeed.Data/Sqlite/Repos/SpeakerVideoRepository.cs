using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class SpeakerVideoRepository : ISpeakerVideoRepository
    {
        public SpeakerVideoRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public ISpeakerVideo Add(ISpeakerVideo entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    context.SpeakerVideo.Add((SpeakerVideo)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(ISpeakerVideo entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var session = context.SpeakerVideo.FirstOrDefault(x => x.Id == entity.Id);
                    if (session != null)
                    {
                        context.Entry(session).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public ISpeakerVideo FindBySpeakerIdAndVideoId(int speakerId, int videoId)
        {
            ISpeakerVideo result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.SpeakerVideo.FirstOrDefault(x => x.SpeakerId == speakerId && x.VideoId == videoId);
                }
            }
            return result;
        }

        public IEnumerable<int> GetSpeakerIdListWhereInVideoIdList(IEnumerable<int> videoIds)
        {
            IEnumerable<int> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.SpeakerVideo.Where(x => videoIds.Contains(x.VideoId)).Select(x => x.SpeakerId).ToList();
                }
            }
            return result;
        }

        public IEnumerable<int> GetVideoIdListWhereInSpeakerIdList(IEnumerable<int> speakerIds)
        {
            IEnumerable<int> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.SpeakerVideo.Where(x => speakerIds.Contains(x.SpeakerId)).Select(x => x.VideoId).ToList();
                }
            }
            return result;
        }
    }
}
