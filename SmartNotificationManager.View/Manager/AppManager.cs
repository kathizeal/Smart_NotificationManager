using WinLogger;
using WinLogger.Contract;

namespace SmartNotificationManager.WinUI.Manager
{
    public class AppManager
    {
        #region AppManagerSingleton class
        /// <summary>Helps getting a singleton instance of <see cref="AppManager"/></summary>
        private class AppManagerSingleton
        {
            // Explicit static constructor
            static AppManagerSingleton() { }
            //Marked as internal as it will be accessed from the enclosing class. It doesn't raise any problem, as the class itself is private.
            internal static readonly AppManager Instance = new AppManager();
        }
        #endregion
        private static ILogger _logger = LogManager.GetLogger();
        internal static AppManager Instance { get { return AppManagerSingleton.Instance; } }
    }
}
