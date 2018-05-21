using System.Collections.Generic;

namespace ChannelNineEventFeed.Library.Intefaces
{
    public interface IAppSettings
    {
        string ConnectionString { get; }
        string VideoFolderName { get; }
        string AppName { get; }

        string LookupConnectionString(string name);
        string LookupAppSetting(string key);
        bool LookupAppSetting(string key, bool defaultValue);
        IEnumerable<string> LookupAppSettingList(string key, string delimiter);
        void CheckForConnectionStringName(string name);
        void CheckForKey(string key);
    }
}