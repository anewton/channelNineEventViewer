using ChannelNineEventFeed.Data.Interfaces;
using System.Threading.Tasks;

namespace ChannelNineEventFeed.Data.Sqlite
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        public DatabaseInitializer(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public async Task InitDatabase(string databaseName)
        {
            Database.DbName = databaseName;
            var databaseExists = await Database.DatabaseExists();
            if (!databaseExists)
            {
                // Create database file and run create scripts
                var fileCreated = await Database.CreateDatabase();
            }
            var databaseObjectsCreated = await Database.CreateDatabaseObjects();
        }

        public async Task RecreateDatabase(string databaseName)
        {
            Database.DbName = databaseName;
            // Create database file and run create scripts
            var fileCreated = await Database.CreateDatabase();         
            var databaseObjectsCreated = await Database.CreateDatabaseObjects();
        }
    }
}
