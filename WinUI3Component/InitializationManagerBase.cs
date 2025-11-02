using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using WinLogger;
using WinLogger.Contract;
using WinLogger.Util;
using WinUI3Component.Util;

namespace WinUI3Component
{
    public abstract class InitializationManagerBase
    {
        protected static ILogger Logger = LogManager.GetLogger();

        protected readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        public bool IsLoggerInitialized { get; private set; }
        protected bool IsPreRequirementsAlreadyInitialized = false;


        #region Abtract Method
        public abstract void InitializeDI();
        protected abstract Task InitializeAppTheme();
        protected abstract Task InitializeLibraryServicesForUser();

        #endregion
        protected virtual void SetCulture()
        {
            var cultureName = (string)WinUI3CommonUtil.GetValueFromLocalSettings("AppCultureInfo", default);
            if (!string.IsNullOrEmpty(cultureName))
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            }
        }
        public virtual void InitializeLogger()
        {
            if (!IsLoggerInitialized)
            {
                LogLevel enabledLogLevel = LogLevel.All;
                //var isPerformanceLoggingEnabled = (bool)UWPCommonUtil.GetValueFromLocalSettings(LoggingConstants.IsPerformanceLoggingEnabledKey, false);
                //var isSQLLoggingEnabled = (bool)UWPCommonUtil.GetValueFromLocalSettings(LoggingConstants.IsSQLLoggingEnabledKey, false); 
                var isPerformanceLoggingEnabled = false;
                var isSQLLoggingEnabled = false;

                LogManager.Instance.Init(LoggingTarget.File, enabledLogLevel, WinUI3CommonUtil.GetLogsFolderPath(), WinUI3CommonUtil.GetLogFileBaseName(),
                    includeCallerDetails: true);
                LogManager.Instance.IsSQLLoggingEnabled = isSQLLoggingEnabled;
                LogManager.Instance.IsPerformanceLoggingEnabled = isPerformanceLoggingEnabled;
                Logger.Debug(LogManager.GetCallerInfo(), "Logger initialized");

                IsLoggerInitialized = true;
            }
        }
        protected virtual async Task InitializePreRequirements()
        {
            try
            {
                InitializeLogger();
                //RegisterLifeCycleEvents();
                //InitializeNetworkConnectivity();
                InitializeDI();
                //await InitializeZSSOProviderAndSSOKit();
                //PopulateMainWindow();
                await InitializeAppTheme();
                SetCulture();
                //await UWPCommonUtil.ParsePrerequisitesFromAppUpdateConfig();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error while initializing pre-rootshell requirements - {ex}");
            }
        }

        public virtual async Task InitializeApp(LaunchActivatedEventArgs args)
        {
            if (!IsPreRequirementsAlreadyInitialized)
            {
                IsPreRequirementsAlreadyInitialized = true;
                await InitializePreRequirements();
                //await CreateRootShell(args);
                //await HandlePreAuthenticationRequirements();
                //await CheckAndLoadUsersIfExist();
                await InitializeLibraryServices();
            }
            else
            {
                //if (CoreApplication.Views.Count > 1)
                //{
                //    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(ViewManager.Instance.MainWindowId, ViewSizePreference.Default);
                //}
                CoreApplication.MainView?.CoreWindow?.Activate();
            }
        }

        private async Task<bool> InitializeLibraryServices()
        {
            try
            {
                await InitializeLibraryServicesForUser();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Debug(LogManager.GetCallerInfo(), $"Error occurred while initialising service dbs {ex}");
                return false;
            }
        }



    }

}
