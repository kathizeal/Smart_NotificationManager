using SmartNotificationLibrary.DBHandler.Contract;
using SmartNotificationLibrary.Domain;
using SmartNotificationManger.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommon.Util;
using WinLogger;
using WinCommon.Extension;

namespace SmartNotificationLibrary.DataManager
{
    internal class GetExcludedPackagesDataManager : SmartNotificationDataManagerBase, IGetExcludedPackagesDataManager
    {
        public GetExcludedPackagesDataManager(INotifyDBHandler dbHandler) : base(dbHandler) { }

        public async Task GetExcludedPackages(GetExcludedPackagesRequest request, ICallback<GetExcludedPackagesResponse> callback)
        {
            try
            {
                IList<KRPackageProfile> excludedPackages = DBHandler.GetExcludedPackagesFromDB(request.UserId);
                var response = new GetExcludedPackagesResponse(excludedPackages);
                callback?.OnCompleted(ResponseType.LocalStorage, response);
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), "Error in Getting Excluded Packages for UserId : {0} Exception : {1}",
                    request.UserId, ex.ToString());
                callback?.OnError(ex);
            }
        }

      
    }
}
