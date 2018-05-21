using ChannelNineEventFeed.Library.Interfaces;

namespace ChannelNineEventFeed.Data.Interfaces
{
    public interface IRepository<T> where T : IDataEntity
    {
        IDatabase Database { get; set; }

        //List<T> PagedList(int take, int skip);

        T Add(T entity);

        //void Delete(T entity);

        void Update(T entity);

        //T FindById(int id);
    }
}
