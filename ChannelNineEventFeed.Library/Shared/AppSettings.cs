using ChannelNineEventFeed.Library.Intefaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ChannelNineEventFeed.Library.Shared
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            ConnectionString = LookupConnectionString("ConnectionString");
            VideoFolderName = LookupAppSetting("VideoFolderName"); // TODO: use System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "My Videos") plus this value to save video files
            AppName = LookupAppSetting("AppName");
        }

        public string ConnectionString { get; private set; }
        public string VideoFolderName { get; private set; }
        public string AppName { get; private set; }

        public string LookupConnectionString(string name)
        {
            CheckForConnectionStringName(name);
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public string LookupAppSetting(string key)
        {
            CheckForKey(key);
            return ConfigurationManager.AppSettings[key].ToString();
        }

        public bool LookupAppSetting(string key, bool defaultValue)
        {
            CheckForKey(key);
            if (!bool.TryParse(ConfigurationManager.AppSettings[key], out bool boolValue))
            {
                boolValue = defaultValue;
            }
            return boolValue;
        }

        public IEnumerable<string> LookupAppSettingList(string key, string delimiter)
        {
            CheckForKey(key);
            var appSettingValue = ConfigurationManager.AppSettings[key].ToString();
            return appSettingValue.Split(delimiter.ToCharArray()).ToList();
        }

        public void CheckForConnectionStringName(string name)
        {
            if (!ConnectionStringNameExists(name))
            {
                throw new Exception(string.Format("Missing config value {0}", name));
            }
        }

        public void CheckForKey(string key)
        {
            if (!AppSettingsKeyExists(key))
            {
                throw new Exception(string.Format("Missing config value {0}", key));
            }
        }

        private bool AppSettingsKeyExists(string key)
        {
            var keyExists = ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings.AllKeys.Where(k => k == key).Count() > 0;
            return keyExists;
        }

        private bool ConnectionStringNameExists(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            return !string.IsNullOrWhiteSpace(connectionString);
        }
    }
}