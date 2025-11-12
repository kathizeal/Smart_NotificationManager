using SmartNotificationLibrary.DBHandler.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinLogger;
using WinLogger.Contract;

namespace SmartNotificationLibrary.DataManager
{
    internal abstract class SmartNotificationDataManagerBase
    {
        protected readonly ILogger Logger = LogManager.GetLogger(); 

        protected INotifyDBHandler DBHandler { get; }

        protected SmartNotificationDataManagerBase(INotifyDBHandler dBHandler)
        {
            DBHandler = dBHandler;

        }
    }
}
