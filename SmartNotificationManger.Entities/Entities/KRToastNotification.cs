using SQLite;
using WinCommon.Util;

namespace SmartNotificationManger.Entities
{
    public class KRToastNotification : ObservableObject
    {
        private DateTimeOffset _createdTime;
        private string _notificationTitle;
        private string _notificationMessage;


        [PrimaryKey]
        public string NotificationId { get; set; }

        public string PackageFamilyName { get; set; }

        public DateTimeOffset CreatedTime
        {
            get => _createdTime;
            set => SetIfDifferent(ref _createdTime, value);
        }
        public string NotificationTitle
        {
            get => _notificationTitle;
            set => SetIfDifferent(ref _notificationTitle, value);
        }

        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetIfDifferent(ref _notificationMessage, value);
        }


        public string DisplayTime => CreatedTime.LocalDateTime.ToString(DateTimeFormatConstant.Format_hh_mm_ss_tt_d_Slash_M);

        public KRToastNotification() { }

        public KRToastNotification DeepClone()
        {
            return new KRToastNotification
            {
                NotificationId = this.NotificationId,
                PackageFamilyName = this.PackageFamilyName,
                CreatedTime = this.CreatedTime,
                NotificationTitle = this.NotificationTitle,
                NotificationMessage = this.NotificationMessage
            };
        }

        public void Update(KRToastNotification newData)
        {
            if (newData != null)
            {
                NotificationId = newData.NotificationId;
                PackageFamilyName = newData.PackageFamilyName;
                CreatedTime = newData.CreatedTime;
                NotificationTitle = newData.NotificationTitle;
                NotificationMessage = newData.NotificationMessage;
            }


        }
    }

}
