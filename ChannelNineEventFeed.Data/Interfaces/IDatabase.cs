using System.Data.SQLite;
using System.Threading.Tasks;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IDatabase
    {
        string DbName { get; set; }
        string DatabasePath { get; }

        Task<bool> DatabaseExists();
        Task<bool> CreateDatabase();
        Task<bool> CreateDatabaseObjects();
        SQLiteConnection DbConnection();
    }
}
