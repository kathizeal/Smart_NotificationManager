using SmartNotificationManger.Entities;
using SmartNotificationManger.Entities.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSQLiteDBAdapter.Contract;

namespace SmartNotificationLibrary.DBHandler
{
    public sealed partial class NotifyDBHandler
    {
        public IList<KRToastNotification> GetToastNotificationByUserId(string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            return dBConnection.Table<KRToastNotification>().ToList();
        }

        public IList<KRToastNotification> GetKToastNotificationsByPackageId(string packageId, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            return dBConnection.Table<KRToastNotification>().Where(x => x.PackageFamilyName == packageId).ToList();
        }

        public void UpdateOrReplaceKToastNotification(ObservableCollection<KRToastNotification> toastNotifications, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            dBConnection.InsertOrReplaceAll(toastNotifications);
        }

        public void UpdateOrReplaceKToastNotification(KRToastBObj toastData, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            dBConnection.RunInTransaction(() => {
                if (toastData != null)
                {
                    if (toastData.NotificationData != null)
                    {
                        dBConnection.InsertOrReplace(toastData.NotificationData, typeof(KRToastNotification));
                    }
                    if (toastData.ToastPackageProfile != null)
                    {
                        dBConnection.InsertOrReplace(toastData.ToastPackageProfile, typeof(KRPackageProfile));
                    }
                }
            });

        }


        public KRPackageProfile GetPackageProfile(string packageFamilyName, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            return dBConnection.Table<KRPackageProfile>().FirstOrDefault(x => x.PackageFamilyName == packageFamilyName);
        }

        public IList<KRPackageProfile> GetKPackageProfiles(string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            return dBConnection.Table<KRPackageProfile>().ToList();
        }


        public IList<KRPackageProfile> GetPackagesBySpaceId(string spaceId, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            var packageFamilyNames = dBConnection.Table<KSpaceMapper>()
                                          .Where(x => x.SpaceId == spaceId)
                                          .Select(x => x.PackageFamilyName)
                                          .ToHashSet<string>();

            return dBConnection.Table<KRPackageProfile>()
                               .Where(x => packageFamilyNames.Contains(x.PackageFamilyName))
                               .ToList();
        }

        public IList<KSpaceMapper> GetAllSpaceMappers(string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            return dBConnection.Table<KSpaceMapper>().ToList();
        }

        public bool AddPackageToSpace(KSpaceMapper mapper, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            dBConnection.InsertOrReplace(mapper);
            return true;
        }

        public void UpdateKPackageProfileFromAddition(KRPackageProfile packageProfile, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            dBConnection.InsertOrReplace(packageProfile);
        }

        public void UpdateSpaces(IEnumerable<KSpace> spaces, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            dBConnection.InsertOrReplaceAll(spaces);
        }

        public bool RemovePackageFromSpace(string spaceId, string packageId, string userId)
        {
            IDBConnection dBConnection = DBAdapter.GetDBConnection(userId);
            var spaceMapper = dBConnection.Table<KSpaceMapper>()
                                          .FirstOrDefault(x => x.SpaceId == spaceId && x.PackageFamilyName == packageId);

            if (spaceMapper != null)
            {
                dBConnection.Delete(spaceMapper);
                return true;
            }
            return false;
        }
    }

}
