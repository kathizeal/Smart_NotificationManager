namespace SmartNotificationLibrary.Enums
{

    public enum Priority
    {
        None, // default
        High,
        Medium,
        Low,
    }

    public enum ViewType
    {
        All = 0,
        Space,
        Package,
        Priority,
        Filters
    }


    public enum NotificatioRequestType
    {
        All,
        Individual
    }
    public enum NotificationSounds
    {
        None,
        // Custom sounds (first 15 - from Assets/NotificationsSounds/)
        Cora,
        Cue,
        Celesta,
        Hail,
        Bell,
        Pac,
        Ting,
        Triangle,
        Wink,
        Heavenly,
        ViolinPing,
        Reverberate,
        Speedy,
        MusicboxDuo,
        MusicboxTriplet,

        // System sounds (from AppNotificationSoundEvent - 16+)
        SystemDefault,
        SystemIM,
        SystemMail,
        SystemReminder,
        SystemSMS,
        SystemAlarm,
        SystemAlarm2,
        SystemAlarm3,
        SystemAlarm4,
        SystemAlarm5,
        SystemAlarm6,
        SystemAlarm7,
        SystemAlarm8,
        SystemAlarm9,
        SystemAlarm10,
        SystemCall,
        SystemCall2,
        SystemCall3,
        SystemCall4,
        SystemCall5,
        SystemCall6,
        SystemCall7,
        SystemCall8,
        SystemCall9,
        SystemCall10,
    }

}
