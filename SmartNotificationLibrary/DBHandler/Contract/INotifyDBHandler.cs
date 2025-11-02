using SmartNotificationLibrary.Enums;
using SmartNotificationManger;
using SmartNotificationManger.Entities;
using SmartNotificationManger.Entities.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommon.Util;
using WinSQLiteDBAdapter.Contract;

namespace SmartNotificationLibrary.DBHandler.Contract
{
    public interface INotifyDBHandler : IDBHandler
    {
        Task InitializeDBAsync(string dbFolderPath, string dbuserId, string dbRefId = null);
        List<Type> GetDBModels();
        List<Type> GetServiceDBModels();
        IList<KRToastNotification> GetToastNotificationByUserId(string userId);
        IList<KRToastNotification> GetKToastNotificationsByPackageId(string packageId, string userId);


        #region PackageProfile

        KRPackageProfile GetPackageProfile(string packageId, string userId);
        IList<KRPackageProfile> GetKPackageProfiles(string userId);


        #endregion
        void UpdateOrReplaceKToastNotification(ObservableCollection<KRToastNotification> toastNotifications, string userId);
        void UpdateOrReplaceKToastNotification(KRToastBObj toastData, string userId);

        void UpdateKPackageProfileFromAddition(KRPackageProfile packageProfile, string userId);

        IList<KRPackageProfile> GetPackagesBySpaceId(string spaceId, string userId);

        IList<KSpaceMapper> GetAllSpaceMappers(string userId);

        bool AddPackageToSpace(KSpaceMapper mapper, string userId);

        bool RemovePackageFromSpace(string spaceId, string packageId, string userId);

        #region Custom Priority Methods

        /// <summary>
        /// Gets all apps with custom priorities for a user
        /// </summary>
        IList<KCustomPriorityApp> GetCustomPriorityApps(string userId);

        /// <summary>
        /// Gets apps by specific priority level
        /// </summary>
        IList<KCustomPriorityApp> GetAppsByPriority(Priority priority, string userId);

        /// <summary>
        /// Adds or updates an app's custom priority
        /// </summary>
        bool AddOrUpdateCustomPriorityApp(string packageId, string displayName, string publisher, Priority priority, string userId);

        /// <summary>
        /// Removes an app from custom priority
        /// </summary>
        bool RemoveCustomPriorityApp(string packageId, string userId);

        /// <summary>
        /// Gets custom priority for a specific app
        /// </summary>
        Priority? GetAppCustomPriority(string packageId, string userId);

        #endregion

        #region Enhanced Space Methods

        /// <summary>
        /// Gets packages in a specific space with their details
        /// </summary>
        IList<KRPackageProfile> GetPackagesBySpaceIdEnhanced(string spaceId, string userId);

        /// <summary>
        /// Adds a package to space with full package profile creation
        /// </summary>
        bool AddPackageToSpaceEnhanced(string packageId, string spaceId, string displayName, string publisher, string userId);

        /// <summary>
        /// Gets space statistics including app and notification counts
        /// </summary>
        Dictionary<string, (int AppCount, int NotificationCount)> GetSpaceStatistics(string userId);

        #endregion

        #region Feedback Methods

        /// <summary>
        /// Submits user feedback to the database
        /// </summary>
        bool SubmitFeedback(string title, string message, FeedbackCategory category, string email, string userId, string appVersion, string osVersion);

        /// <summary>
        /// Gets all feedback for a user
        /// </summary>
        IList<KFeedback> GetUserFeedback(string userId);

        /// <summary>
        /// Gets feedback by category for a user
        /// </summary>
        IList<KFeedback> GetFeedbackByCategory(FeedbackCategory category, string userId);

        /// <summary>
        /// Updates feedback status
        /// </summary>
        bool UpdateFeedbackStatus(string feedbackId, FeedbackStatus status, string userId);

        #endregion

        #region Sound Mapping Methods

        /// <summary>
        /// Gets all sound mappings for a user
        /// </summary>
        IList<KSoundMapper> GetSoundMappings(string userId);

        /// <summary>
        /// Gets sound mapping for a specific package
        /// </summary>
        NotificationSounds GetPackageSound(string packageFamilyName, string userId);

        /// <summary>
        /// Adds or updates sound mapping for a package
        /// </summary>
        bool AddOrUpdateSoundMapping(string packageFamilyName, NotificationSounds sound, string userId);

        /// <summary>
        /// Removes sound mapping for a package (resets to None)
        /// </summary>
        bool RemoveSoundMapping(string packageFamilyName, string userId);

        /// <summary>
        /// Gets packages grouped by their assigned sound
        /// </summary>
        Dictionary<NotificationSounds, List<string>> GetPackagesBySound(string userId);

        #endregion

        #region Notification Management Methods

        /// <summary>
        /// Removes all notifications for a specific package
        /// </summary>
        bool ClearPackageNotifications(string packageFamilyName, string userId);

        /// <summary>
        /// Gets notification count for a specific package
        /// </summary>
        int GetPackageNotificationCount(string packageFamilyName, string userId);

        #endregion
    }


}
