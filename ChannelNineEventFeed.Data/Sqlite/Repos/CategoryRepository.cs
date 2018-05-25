using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ChannelNineEventFeed.Data.Sqlite.Repos
{
    public class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; set; }

        public ICategory Add(ICategory entity)
        {
            var existingCategory = FindByName(entity.Name);
            if (existingCategory != null)
            {
                return existingCategory;
            }
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {

                    context.Category.Add((Category)entity);
                    context.SaveChanges();
                }
            }
            return entity;
        }

        public void Update(ICategory entity)
        {
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    var category = context.Category.FirstOrDefault(x => x.Id == entity.Id);
                    if (category != null)
                    {
                        context.Entry(category).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                }
            }
        }

        public ICategory FindByName(string name)
        {
            ICategory result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Category.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                }
            }
            return result;
        }

        public IEnumerable<ICategory> GetCategoriesInSessionCategoryIdList(IEnumerable<int> sessionCategoryIds)
        {
            IEnumerable<ICategory> result = null;
            using (var conn = new SQLiteConnection(Database.DatabasePath))
            {
                using (var context = new DatabaseContext(conn))
                {
                    result = context.Category.Where(x => sessionCategoryIds.Contains(x.Id)).ToList();
                }
            }
            return result;
        }
    }
}
