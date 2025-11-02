using Microsoft.Windows.AppNotifications.Builder;
using SmartNotificationLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationLibrary.Services
{
    /// <summary>
    /// Helper class for notification sound management
    /// Handles both custom sounds (SetAudioUri) and system sounds (SetAudioEvent)
    /// </summary>
    public static class NotificationSoundHelper
    {
        // First 15 sounds are custom sounds from Assets
        private const int CustomSoundsThreshold = 15;

        /// <summary>
        /// Determines if a sound is a custom sound (should use SetAudioUri) or system sound (should use SetAudioEvent)
        /// </summary>
        public static bool IsCustomSound(NotificationSounds sound)
        {
            return (int)sound > 0 && (int)sound <= CustomSoundsThreshold;
        }

        /// <summary>
        /// Determines if a sound is a system sound (should use SetAudioEvent)
        /// </summary>
        public static bool IsSystemSound(NotificationSounds sound)
        {
            return (int)sound > CustomSoundsThreshold;
        }

        /// <summary>
        /// Gets the file path for custom notification sounds
        /// </summary>
        public static string GetCustomSoundPath(NotificationSounds sound)
        {
            if (!IsCustomSound(sound))
            {
                return "ms-winsoundevent:Notification.Default";
            }

            try
            {
                // Use ms-appx URI for bundled sound files
                var soundFileName = $"{sound}.wav";
                return $"ms-appx:///Assets/NotificationsSounds/{soundFileName}";
            }
            catch (Exception)
            {
                return "ms-winsoundevent:Notification.Default";
            }
        }

        /// <summary>
        /// Maps NotificationSounds to AppNotificationSoundEvent for system sounds
        /// </summary>
        public static AppNotificationSoundEvent GetSystemSoundEvent(NotificationSounds sound)
        {
            return sound switch
            {
                NotificationSounds.SystemDefault => AppNotificationSoundEvent.Default,
                NotificationSounds.SystemIM => AppNotificationSoundEvent.IM,
                NotificationSounds.SystemMail => AppNotificationSoundEvent.Mail,
                NotificationSounds.SystemReminder => AppNotificationSoundEvent.Reminder,
                NotificationSounds.SystemSMS => AppNotificationSoundEvent.SMS,
                NotificationSounds.SystemAlarm => AppNotificationSoundEvent.Alarm,
                NotificationSounds.SystemAlarm2 => AppNotificationSoundEvent.Alarm2,
                NotificationSounds.SystemAlarm3 => AppNotificationSoundEvent.Alarm3,
                NotificationSounds.SystemAlarm4 => AppNotificationSoundEvent.Alarm4,
                NotificationSounds.SystemAlarm5 => AppNotificationSoundEvent.Alarm5,
                NotificationSounds.SystemAlarm6 => AppNotificationSoundEvent.Alarm6,
                NotificationSounds.SystemAlarm7 => AppNotificationSoundEvent.Alarm7,
                NotificationSounds.SystemAlarm8 => AppNotificationSoundEvent.Alarm8,
                NotificationSounds.SystemAlarm9 => AppNotificationSoundEvent.Alarm9,
                NotificationSounds.SystemAlarm10 => AppNotificationSoundEvent.Alarm10,
                NotificationSounds.SystemCall => AppNotificationSoundEvent.Call,
                NotificationSounds.SystemCall2 => AppNotificationSoundEvent.Call2,
                NotificationSounds.SystemCall3 => AppNotificationSoundEvent.Call3,
                NotificationSounds.SystemCall4 => AppNotificationSoundEvent.Call4,
                NotificationSounds.SystemCall5 => AppNotificationSoundEvent.Call5,
                NotificationSounds.SystemCall6 => AppNotificationSoundEvent.Call6,
                NotificationSounds.SystemCall7 => AppNotificationSoundEvent.Call7,
                NotificationSounds.SystemCall8 => AppNotificationSoundEvent.Call8,
                NotificationSounds.SystemCall9 => AppNotificationSoundEvent.Call9,
                NotificationSounds.SystemCall10 => AppNotificationSoundEvent.Call10,
                _ => AppNotificationSoundEvent.Default
            };
        }

        /// <summary>
        /// Gets display text for both custom and system sounds
        /// </summary>
        public static string GetSoundDisplayText(NotificationSounds sound)
        {
            return sound switch
            {
                NotificationSounds.None => "Default",

                // Custom sounds
                NotificationSounds.Cora => "Cora",
                NotificationSounds.Cue => "Cue",
                NotificationSounds.Celesta => "Celesta",
                NotificationSounds.Hail => "Hail",
                NotificationSounds.Bell => "Bell",
                NotificationSounds.Pac => "Pac",
                NotificationSounds.Ting => "Ting",
                NotificationSounds.Triangle => "Triangle",
                NotificationSounds.Wink => "Wink",
                NotificationSounds.Heavenly => "Heavenly",
                NotificationSounds.ViolinPing => "Violin Ping",
                NotificationSounds.Reverberate => "Reverberate",
                NotificationSounds.Speedy => "Speedy",
                NotificationSounds.MusicboxDuo => "Musicbox Duo",
                NotificationSounds.MusicboxTriplet => "Musicbox Triplet",

                // System sounds
                NotificationSounds.SystemDefault => "System Default",
                NotificationSounds.SystemIM => "System IM",
                NotificationSounds.SystemMail => "System Mail",
                NotificationSounds.SystemReminder => "System Reminder",
                NotificationSounds.SystemSMS => "System SMS",
                NotificationSounds.SystemAlarm => "System Alarm",
                NotificationSounds.SystemAlarm2 => "System Alarm 2",
                NotificationSounds.SystemAlarm3 => "System Alarm 3",
                NotificationSounds.SystemAlarm4 => "System Alarm 4",
                NotificationSounds.SystemAlarm5 => "System Alarm 5",
                NotificationSounds.SystemAlarm6 => "System Alarm 6",
                NotificationSounds.SystemAlarm7 => "System Alarm 7",
                NotificationSounds.SystemAlarm8 => "System Alarm 8",
                NotificationSounds.SystemAlarm9 => "System Alarm 9",
                NotificationSounds.SystemAlarm10 => "System Alarm 10",
                NotificationSounds.SystemCall => "System Call",
                NotificationSounds.SystemCall2 => "System Call 2",
                NotificationSounds.SystemCall3 => "System Call 3",
                NotificationSounds.SystemCall4 => "System Call 4",
                NotificationSounds.SystemCall5 => "System Call 5",
                NotificationSounds.SystemCall6 => "System Call 6",
                NotificationSounds.SystemCall7 => "System Call 7",
                NotificationSounds.SystemCall8 => "System Call 8",
                NotificationSounds.SystemCall9 => "System Call 9",
                NotificationSounds.SystemCall10 => "System Call 10",

                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets sound type description for UI
        /// </summary>
        public static string GetSoundTypeDescription(NotificationSounds sound)
        {
            if (sound == NotificationSounds.None)
                return "Default";

            if (IsCustomSound(sound))
                return "Custom";

            if (IsSystemSound(sound))
                return "System";

            return "Unknown";
        }
    }
}
