using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Intefaces;
using Dapper;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace ChannelNineEventFeed.Data.Sqlite
{
    public class Database : IDatabase
    {
        private IAppSettings _appSettings;

        public Database(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string DbName { get; set; }

        public string DatabasePath
        {
            get
            {
                return GetDatabaseFilePath();
            }
        }

        public SQLiteConnection DbConnection()
        {
            var filePath = GetDatabaseFilePath();
            return new SQLiteConnection("Data Source=" + filePath);
        }

        public Task<bool> DatabaseExists()
        {
            return Task.Factory.StartNew(() =>
            {
                var filePath = GetDatabaseFilePath();
                return System.IO.File.Exists(filePath);
            });
        }

        public Task<bool> CreateDatabase()
        {
            return Task.Factory.StartNew(() =>
            {
                var fileCreated = false;
                try
                {
                    var filePath = GetDatabaseFilePath();
                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        fileCreated = System.IO.File.Exists(filePath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return fileCreated;
            });
        }

        public Task<bool> CreateDatabaseObjects()
        {
            return Task.Factory.StartNew(() =>
            {
                var result = false;
                var tables = CreateTables.GetAll();
                if (tables != null)
                {
                    foreach (var script in tables)
                    {
                        using (var cnn = DbConnection())
                        {
                            cnn.Open();
                            try
                            {
                                cnn.Execute(script.Sql);
                                result = true;
                            }
                            catch (Exception)
                            {
                                result = false;
                            }
                            cnn.Close();
                        }
                    }
                }
                return result;
            });
        }

        private string GetDatabaseFilePath()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            var connectionString = _appSettings.ConnectionString;
            var filePath = System.IO.Path.Combine(directory, connectionString);
            return filePath;
        }
    }
}
