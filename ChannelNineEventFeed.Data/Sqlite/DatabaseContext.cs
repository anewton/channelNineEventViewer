using ChannelNineEventFeed.Library.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace ChannelNineEventFeed.Data.Sqlite
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(SQLiteConnection connection) //: // base(connection, true)
           : base(
                new SQLiteConnection()
        {
            ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = connection.ConnectionString, ForeignKeys = true }.ConnectionString
            }, true)
        {
            System.Data.Entity.Database.SetInitializer<DatabaseContext>(null);
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<SessionCategory> SessionCategory { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Speaker> Speaker { get; set; }
        public DbSet<SpeakerVideo> SpeakerVideo { get; set; }
        public DbSet<Video> Video { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
