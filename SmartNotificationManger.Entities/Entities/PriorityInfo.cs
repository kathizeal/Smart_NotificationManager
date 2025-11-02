using SmartNotificationLibrary.Enums;

namespace SmartNotificationManger.Entities
{
    public class PriorityInfo
    {
        public bool HasPriority { get; set; }
        public Priority Priority { get; set; }
        public string PriorityText { get; set; } = "";
        public string PriorityIcon { get; set; } = "";
    }

}
