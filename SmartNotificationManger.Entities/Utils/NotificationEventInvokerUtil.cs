using SmartNotificationManger.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationManger.Entities.Utils
{
    public static class NotificationEventInokerUtil
    {
        public static event Action<NotificationReceivedEventArgs> NotificationReceived;

        public static void NotifyNotificationListened(NotificationReceivedEventArgs eventArgs)
        {
            NotificationReceived?.Invoke(eventArgs);
        }

    }

    public class NotificationReceivedEventArgs : EventArgs
    {
        public KRToastBObj Notification { get; }

        public NotificationReceivedEventArgs(KRToastBObj notification)
        {
            Notification = notification;
        }
    }
}
