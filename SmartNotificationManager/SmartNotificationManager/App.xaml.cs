using Microsoft.UI;
using Microsoft.UI.Xaml;
using SmartNotificationManager.View.Services;
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
        private BackgroundNotificationService? _backgroundService;



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

            try
            {
                Logger.Info(LogManager.GetCallerInfo(), "Application launched");

                // Initialize library services for the current user
                InitializeLibraryServices();

                // Initialize background services first
                InitializeBackgroundServices();

                //// Create and show main window
                //m_window = new MainWindow();
                //m_window.Closed += OnMainWindowClosed;

                //// Set the main window reference for toast activation
                //ToastActivationService.SetMainWindow(m_window);

                //m_window.Activate();

                _window = new MainWindow();
                _window.Activate();


                Logger.Info(LogManager.GetCallerInfo(), "Main window created and activated");
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error during application launch: {ex.Message}");
            }
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

        /// <summary>
        /// Initializes library services for the current user
        /// </summary>
        private async void InitializeLibraryServices()
        {
            try
            {
                Logger.Info(LogManager.GetCallerInfo(), "Initializing library services");

                // Initialize library services (database, etc.)
                await InitializationManager.Instance.InitializeLibraryServicesForCurrentUser();

                Logger.Info(LogManager.GetCallerInfo(), "Library services initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error initializing library services: {ex.Message}");
            }
        }

        private async void InitializeBackgroundServices()
        {
            try
            {
//                // Run tray diagnostics first (only in debug mode)
//#if DEBUG
//                Debug.WriteLine("Running tray diagnostics...");
//                await TrayDiagnostics.RunTrayDiagnosticsAsync();
//                TrayDiagnostics.CheckNotificationAreaSettings();
//#endif

//                // Initialize tray manager first
//                _trayManager = new TrayManager();
//                _trayManager.ShowMainWindowRequested += OnShowMainWindowRequested;
//                _trayManager.ExitApplicationRequested += OnExitApplicationRequested;

//                try
//                {
//                    _trayManager.Initialize();
//                    Logger.Info(LogManager.GetCallerInfo(), "Tray manager initialized successfully");

//                    // Test the tray icon by showing a welcome notification
//                    _trayManager.ShowBalloonTip("INotify Started", "INotify is now running in the system tray. Click the icon to show the window.");
//                }
//                catch (Exception trayEx)
//                {
//                    Logger.Warning(LogManager.GetCallerInfo(), $"Tray manager initialization had issues but continuing: {trayEx.Message}");

//                    // Run quick tray test to help diagnose the issue
//#if DEBUG
//                    Debug.WriteLine("Running quick tray test due to initialization issues...");
//                    await TrayDiagnostics.QuickTrayTestAsync();
//#endif
//                    // Continue without tray functionality if it fails
//                }

                // Initialize background notification service
                _backgroundService = new BackgroundNotificationService();
                bool started = await _backgroundService.StartAsync();

                if (started)
                {
                    Logger.Info(LogManager.GetCallerInfo(), "Background notification service started successfully");
                }
                else
                {
                    Logger.Warning(LogManager.GetCallerInfo(), "Failed to start background notification service");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error initializing background services: {ex.Message}");
            }
        }


    }
}
