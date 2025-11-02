using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSQLiteDBAdapter.Contract;

namespace SmartNotificationLibrary.DBHandler.Contract
{
    public interface INotifyDBHandler : IDBHandler
    {
        Task InitializeDBAsync(string dbFolderPath, string dbuserId, string dbRefId = null);
        List<Type> GetDBModels();
        List<Type> GetServiceDBModels();

    }

}
