using WinCommon.Util;

namespace SmartNotificationManger.Entities
{
    /// <summary>
    /// Represents user feedback for the application
    /// </summary>
    public class KFeedback
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public FeedbackCategory Category { get; set; }
        public string Email { get; set; }
        public FeedbackStatus Status { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
        public string AppVersion { get; set; }
        public string OSVersion { get; set; }

        public KFeedback()
        {
            Id = Guid.NewGuid().ToString();
            Status = FeedbackStatus.Submitted;
            CreatedTime = DateTimeOffset.Now;
            UpdatedTime = DateTimeOffset.Now;
        }
    }
}
