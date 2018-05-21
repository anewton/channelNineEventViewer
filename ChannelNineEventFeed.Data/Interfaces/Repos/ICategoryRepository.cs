using ChannelNineEventFeed.Library.Interfaces;
using System.Collections.Generic;

namespace ChannelNineEventFeed.Data.Interfaces.Repos
{
    public interface ICategoryRepository : IRepository<ICategory>
    {
        ICategory FindByName(string name);
        IEnumerable<ICategory> GetCategoriesInSessionCategoryIdList(IEnumerable<int> sessionCategoryIds);
    }
}