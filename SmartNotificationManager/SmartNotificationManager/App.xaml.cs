using Microsoft.UI.Xaml;
using SmartNotificationManager.WinUI.Manager;
using System;
using WinCommon.Util;
using WinLogger;
using WinLogger.Contract;
using WinUI3Component.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartNotificationManager
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeDependencyInjection();
            InitializeComponent();
            WinUI3AppInfo.Initialize("INotify");
            ZAppInfoProvider.Initialize(WinUI3AppInfo.Instance);

            Logger.Info(LogManager.GetCallerInfo(), "App constructor called");

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private void InitializeDependencyInjection()
        {
            try
            {
                Logger.Info(LogManager.GetCallerInfo(), "Initializing dependency injection services");

                // Initialize the main DI container
                InitializationManager.Instance.InitializeDI();

                Logger.Info(LogManager.GetCallerInfo(), "Dependency injection services initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error initializing dependency injection: {ex.Message}");
                // Don't throw here as we want the app to continue even if DI fails
            }
        }

    }
}
