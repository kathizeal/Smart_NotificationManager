using SmartNotificationLibrary.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationManger.Entities
{
    public class KSoundMapper
    {
        [PrimaryKey]
        public string PackageFamilyName { get; set; } = string.Empty;

        /// <summary>
        /// Associated notification sound for this package
        /// </summary>
        public NotificationSounds Sound { get; set; } = NotificationSounds.None;

        /// <summary>
        /// User who owns this sound mapping
        /// </summary>
        [Indexed]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// When this sound mapping was created
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// When this sound mapping was last updated
        /// </summary>
        public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Whether this sound mapping is active
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}
