using ChannelNineEventFeed.Data.Sqlite.Scripts;
using System.Collections.Generic;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite
{
    public class CreateTables
    {
        public static readonly CreateTables Category = new CreateTables(Create.CategoryTable, 1);
        public static readonly CreateTables Media = new CreateTables(Create.MediaTable, 2);
        public static readonly CreateTables Session = new CreateTables(Create.SessionTable, 3);
        public static readonly CreateTables Speaker = new CreateTables(Create.SpeakerTable, 4);
        public static readonly CreateTables Video = new CreateTables(Create.VideoTable, 5);
        public static readonly CreateTables SessionCategory = new CreateTables(Create.SessionCategoryTable, 6);
        public static readonly CreateTables SpeakerVideo = new CreateTables(Create.SpeakerVideoTable, 7);

        private static List<CreateTables> AllScripts;

        private readonly int _order;

        private CreateTables(string sql, int order = 0)
        {
            Sql = sql;
            _order = order;
            if (AllScripts == null)
            {
                AllScripts = new List<CreateTables>();
            }
            AllScripts.Add(this);
        }

        public string Sql { get; set; }

        public static List<CreateTables> GetAll()
        {
            if (AllScripts != null)
            {
                return AllScripts.OrderBy(x => x._order).ToList();
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Sql;
        }
    }
}
