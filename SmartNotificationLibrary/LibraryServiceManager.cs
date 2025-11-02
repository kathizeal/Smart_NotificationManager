using Microsoft.Extensions.DependencyInjection;
using SmartNotificationLibrary.DBHandler.Contract;
using SmartNotificationLibrary.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinLogger;
using WinLogger.Contract;

namespace SmartNotificationLibrary
{
    public static class LibraryServiceManager
    {
        static ILogger Logger = LogManager.GetLogger();
        public static void InitializeDI(IServiceCollection serviceCollection)
        {
            NotifyLibraryDIServiceProvider.Instance.Initialize(serviceCollection);
        }

        public static async Task IntializeDB(string dbFolderPath, string userId, string dbRefId = default)
        {
            INotifyDBHandler dBHandler = NotifyLibraryDIServiceProvider.Instance.GetService<INotifyDBHandler>();
            Logger.Info(LogManager.GetCallerInfo(), "Initializing DB for Service");
            await dBHandler.InitializeDBAsync(dbFolderPath, userId, dbRefId).ConfigureAwait(false);
            Logger.Info(LogManager.GetCallerInfo(), "Initialized DB for Service");
        }
    }
}
