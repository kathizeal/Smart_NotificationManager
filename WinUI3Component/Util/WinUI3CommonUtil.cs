using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommon.Util;
using Windows.Storage;

namespace WinUI3Component.Util
{
    public static class WinUI3CommonUtil
    {
        public static string GetLogsFolderPath()
        {
            var localFolderPath = ApplicationData.Current.LocalFolder.Path;
            return Path.Combine(localFolderPath, ZAppInfoProvider.ZAppInfo.AppName, "Logs");
        }

        public static string GetLogFileBaseName()
        {
            return $"{ZAppInfoProvider.ZAppInfo.AppDisplayName}";
        }
        private static ApplicationDataContainer GetApplicationDataContainer(string container)
        {
            return ApplicationData.Current.LocalSettings.CreateContainer(string.IsNullOrEmpty(container)
                ? ZAppInfoProvider.ZAppInfo.AppName : container, ApplicationDataCreateDisposition.Always);
        }


        public static object GetValueFromLocalSettings(string key, object defaultValue = null, string container = null)
        {
            var localSettingsContainer = GetApplicationDataContainer(container);
            return (localSettingsContainer.Values.ContainsKey(key) ? localSettingsContainer.Values[key] : defaultValue);
        }
        public static string GetRootDBFolderPath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, ZAppInfoProvider.ZAppInfo.AppName);
        }
    }
}
