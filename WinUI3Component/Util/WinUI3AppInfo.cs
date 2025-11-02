using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommon.Util;
using Windows.ApplicationModel;

namespace WinUI3Component.Util
{
    public sealed class WinUI3AppInfo : ZAppInfo
    {
        #region PrivateMemebers

        private static bool _Initialized;

        /// <summary>Private constructor to make it a singleton class</summary>
        private WinUI3AppInfo()
        {
            if (!_Initialized) { throw new InvalidOperationException("Not initialized"); }
            AppId = _AppId;
            AppName = _AppName;
            AppDisplayName = _AppDisplayName;
            CurrentVersion = _CurrentVersion;
            PreviousVersion = _PreviousVersion;
            Description = _Description;
            IsRailTimeFormat = _IsRailTimeFormat;
        }

        #endregion
        public static WinUI3AppInfo Instance { get { return AppInfoSingleton.Instance; } }

        private static string _AppId { get; set; }

        /// <summary>Display name of the app</summary>
        private static string _AppName { get; set; }

        private static string _AppDisplayName { get; set; }

        private static Version _CurrentVersion { get; set; }

        private static Version _PreviousVersion { get; set; }

        private static string _Description { get; set; }

        private static bool _IsRailTimeFormat { get; set; }
        public override string CurrentCultureName { get; set; }
        public static void Initialize(string appName, string appDisplayName = default)
        {
            #region Application Identity
            _AppName = appName;
            Package appPackage = Package.Current;
            _AppId = appPackage.Id.Name;
            _Description = appPackage.Description;
            _AppDisplayName = string.IsNullOrWhiteSpace(appDisplayName) ? appPackage.DisplayName : appDisplayName;
            #endregion

            #region Version Assigning

            SetAppVersion();

            #endregion

            #region TimeFormat
            //var regionInfo = new RegionInfo(GlobalizationPreferences.Languages.First());
            //var timeFormat = new DateTimeFormatter("shorttime", new[] { regionInfo.TwoLetterISORegionName });
            _IsRailTimeFormat = false;
            #endregion

            _Initialized = true;
        }

        private static void SetAppVersion()
        {
            Package appPackage = Package.Current;

            //Current Version
            var packageVersion = appPackage.Id.Version;
            _CurrentVersion = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);

            //Previous Version
            //Common Util also AppInfo as Default param to avoid circular dependency app name as Container name is passed as param
            string previousVersion = (string)WinUI3CommonUtil.GetValueFromLocalSettings(PreviousAppVersionKey, "", container: _AppName);
            _PreviousVersion = !string.IsNullOrEmpty(previousVersion) ? new Version(previousVersion) : _CurrentVersion;
        }
        #region AppInfoSingleton class
        private class AppInfoSingleton
        {
            // Explicit static constructor
            static AppInfoSingleton() { }

            //Marked as internal as it will be accessed from the enclosing class. It doesn't raise any problem, as the class itself is private.
            internal static readonly WinUI3AppInfo Instance = new WinUI3AppInfo();
        }
        #endregion

        public override bool IsFreshInstallation()
        {
            string previousVersion = (string)WinUI3CommonUtil.GetValueFromLocalSettings(PreviousAppVersionKey, string.Empty);
            return string.IsNullOrEmpty(previousVersion);
        }
    }

}
