using WinCommon.Util;

namespace SmartNotificationManger.Entities.Model
{
    public class KRToastBObj : ObservableObject
    {
        private KRToastNotification _NotificationData;

        public KRToastNotification NotificationData
        {
            get { return _NotificationData; }
            private set { SetIfDifferent(ref _NotificationData, value); }
        }

        private KRPackageProfile _toastPackageProfile;
        public KRPackageProfile ToastPackageProfile
        {
            get { return _toastPackageProfile; }
            private set { SetIfDifferent(ref _toastPackageProfile, value); }
        }

        public KRToastBObj(KRToastNotification notificationData, KRPackageProfile packageProfile)
        {
            NotificationData = notificationData;
            ToastPackageProfile = packageProfile;
        }
        public KRToastBObj DeepClone()
        {
            return new KRToastBObj(
                NotificationData.DeepClone(),
                ToastPackageProfile.DeepClone()
            );
        }

        public KRToastBObj()
        {

        }

        public void Update(KRToastBObj newData)
        {
            if (newData == null) { return; }

            NotificationData.Update(newData.NotificationData);
            ToastPackageProfile.Update(newData.ToastPackageProfile);
        }
    }
}
