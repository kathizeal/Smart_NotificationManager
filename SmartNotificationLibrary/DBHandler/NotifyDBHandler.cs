using SmartNotificationLibrary.DBHandler.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinLogger;
using WinSQLiteDBAdapter.Contract;

namespace SmartNotificationLibrary.DBHandler
{
    public sealed partial class NotifyDBHandler : DBHandlerBase, INotifyDBHandler
    {

        public NotifyDBHandler(IDBAdapter dbAdapter) : base(dbAdapter) { }

        public List<Type> GetDBModels()
        {
            List<Type> dbModels = new()
            {
                //typeof(KToastNotification),
                //typeof(KPackageProfile),
                //typeof(KSpace),
                //typeof(KSpaceMapper),
                //typeof(KCustomPriorityApp), // Add custom priority model
                //typeof(KFeedback), // Add feedback model
                //typeof(KSoundMapper) // Add sound mapping model
            };
            return dbModels;
        }

        public async Task InitializeDBAsync(string dbFolderPath, string dbuserId, string dbRefId = null)
        {
            try
            {
                await InitializeDBAdapterAsync(dbFolderPath).ConfigureAwait(false);

                IDBConnection DBConnection = await DBAdapter.CreateOrGetDBConnectionAsync(dbuserId, dbRefId).ConfigureAwait(false);
                DBConnection.CreateTables(GetDBModels());
            }

            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
            }
        }

        public override async System.Threading.Tasks.Task InitializeDBAdapterAsync(string dbFolderPath, bool isReadOnlyConn = false)
        {
            if (!DBAdapter.IsInitialized)
            {
                await DBAdapter.InitializeAsync(dbFolderPath, "ToastData").ConfigureAwait(false);

                IDBConnection serviceDbConn = DBAdapter.GetDBConnection();
                serviceDbConn.CreateTables(GetServiceDBModels());
            }
        }
    }
}
