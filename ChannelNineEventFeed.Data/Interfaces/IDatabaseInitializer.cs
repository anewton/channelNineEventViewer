using System.Threading.Tasks;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IDatabaseInitializer
    {
        IDatabase Database { get; set; }
        Task InitDatabase(string databaseName);
    }
}
