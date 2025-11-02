using SmartNotificationLibrary.DBHandler.Contract;
using SmartNotificationLibrary.Enums;
using SmartNotificationManger.Entities;
using SmartNotificationManger.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinCommon.Util;
using Windows.UI.Notifications;
using WinLogger;
using WinSQLiteDBAdapter.Contract;

namespace SmartNotificationLibrary.DBHandler
{
    public sealed partial class NotifyDBHandler : DBHandlerBase, INotifyDBHandler
    {

        public NotifyDBHandler(IDBAdapter dbAdapter) : base(dbAdapter) { }

        public List<Type> GetDBModels()
        {
            List<Type> dbModels = new()
            {
                typeof(KRToastNotification),
                typeof(KRPackageProfile),
                typeof(KSpace),
                typeof(KSpaceMapper),
                typeof(KCustomPriorityApp), // Add custom priority model
                typeof(KFeedback), // Add feedback model
                typeof(KSoundMapper) // Add sound mapping model
            };
            return dbModels;
        }

        public async Task InitializeDBAsync(string dbFolderPath, string dbuserId, string dbRefId = null)
        {
            try
            {
                await InitializeDBAdapterAsync(dbFolderPath).ConfigureAwait(false);

                IDBConnection DBConnection = await DBAdapter.CreateOrGetDBConnectionAsync(dbuserId, dbRefId).ConfigureAwait(false);
                DBConnection.CreateTables(GetDBModels());
            }

            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
            }
        }

        public override async System.Threading.Tasks.Task InitializeDBAdapterAsync(string dbFolderPath, bool isReadOnlyConn = false)
        {
            if (!DBAdapter.IsInitialized)
            {
                await DBAdapter.InitializeAsync(dbFolderPath, "ToastData").ConfigureAwait(false);

                IDBConnection serviceDbConn = DBAdapter.GetDBConnection();
                serviceDbConn.CreateTables(GetServiceDBModels());
            }
        }


        #region Custom Priority Methods

        /// <summary>
        /// Gets all apps with custom priorities for a user
        /// </summary>
        public IList<KCustomPriorityApp> GetCustomPriorityApps(string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                return dbConnection.Table<KCustomPriorityApp>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KCustomPriorityApp>();
            }
        }

        /// <summary>
        /// Gets apps by specific priority level
        /// </summary>
        public IList<KCustomPriorityApp> GetAppsByPriority(Priority priority, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                return dbConnection.Table<KCustomPriorityApp>()
                    .Where(x => x.Priority == priority)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KCustomPriorityApp>();
            }
        }

        /// <summary>
        /// Adds or updates an app's custom priority
        /// </summary>
        public bool AddOrUpdateCustomPriorityApp(string packageName, string displayName, string publisher, Priority priority, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                // Check if app already exists
                var existingApp = dbConnection.Table<KCustomPriorityApp>()
                    .FirstOrDefault(x => x.PackageFamilyName == packageName);

                if (existingApp != null)
                {
                    // Update existing
                    existingApp.Priority = priority;
                    dbConnection.InsertOrReplace(existingApp);
                }
                else
                {
                    // Create new
                    var newApp = new KCustomPriorityApp
                    {
                        Id = Guid.NewGuid().ToString(),
                        PackageFamilyName = packageName,
                        Priority = priority,
                    };

                    dbConnection.InsertOrReplace(newApp);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Removes an app from custom priority
        /// </summary>
        public bool RemoveCustomPriorityApp(string packageId, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                var result = dbConnection.Execute(
                    $"DELETE FROM {nameof(KCustomPriorityApp)}  WHERE {nameof(KCustomPriorityApp.PackageFamilyName)} = ?",
                    packageId, userId);

                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets custom priority for a specific app
        /// </summary>
        public Priority? GetAppCustomPriority(string packageId, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                var app = dbConnection.Table<KCustomPriorityApp>()
                    .FirstOrDefault(x => x.PackageFamilyName == packageId);

                return app?.Priority;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return null;
            }
        }

        #endregion

        #region Enhanced Space Methods

        /// <summary>
        /// Gets packages in a specific space with their details
        /// </summary>
        public IList<KRPackageProfile> GetPackagesBySpaceIdEnhanced(string spaceId, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                // Get package IDs from space mapper
                var packageFamilyNames = dbConnection.Table<KSpaceMapper>()
                    .Where(x => x.SpaceId == spaceId)
                    .Select(x => x.PackageFamilyName)
                    .ToList();

                if (!packageFamilyNames.Any())
                    return new List<KRPackageProfile>();

                // Get package profiles
                var packages = new List<KRPackageProfile>();
                foreach (var packageFamilyName in packageFamilyNames)
                {
                    var package = dbConnection.Table<KRPackageProfile>()
                        .FirstOrDefault(x => x.PackageFamilyName == packageFamilyName);
                    if (package != null)
                    {
                        packages.Add(package);
                    }
                }

                return packages;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KRPackageProfile>();
            }
        }

        /// <summary>
        /// Adds a package to space with full package profile creation
        /// </summary>
        public bool AddPackageToSpaceEnhanced(string packageFamilyName, string spaceId, string displayName, string publisher, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                dbConnection.RunInTransaction(() =>
                {
                    // Ensure package profile exists
                    var existingProfile = dbConnection.Table<KRPackageProfile>()
                        .FirstOrDefault(x => x.PackageFamilyName == packageFamilyName);

                    if (existingProfile == null)
                    {
                        var newProfile = new KRPackageProfile
                        {
                            PackageFamilyName = packageFamilyName,
                            AppDisplayName = displayName,
                            AppDescription = $"Application: {displayName}",
                        };
                        dbConnection.InsertAll(new[] { newProfile });
                    }

                    // Check if mapping already exists
                    var existingMapper = dbConnection.Table<KSpaceMapper>()
                        .FirstOrDefault(x => x.PackageFamilyName == packageFamilyName && x.SpaceId == spaceId);

                    if (existingMapper == null)
                    {
                        var mapper = new KSpaceMapper(spaceId, packageFamilyName)
                        {
                            PackageFamilyName = packageFamilyName,
                            SpaceId = spaceId
                        };
                        dbConnection.InsertAll(new[] { mapper });
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets space statistics including app and notification counts
        /// </summary>
        public Dictionary<string, (int AppCount, int NotificationCount)> GetSpaceStatistics(string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                var stats = new Dictionary<string, (int AppCount, int NotificationCount)>();

                var spaces = dbConnection.Table<KSpace>().ToList();

                foreach (var space in spaces)
                {
                    var appCount = dbConnection.Table<KSpaceMapper>()
                        .Count(x => x.SpaceId == space.SpaceId);

                    // Get notification count for packages in this space
                    var packageIds = dbConnection.Table<KSpaceMapper>()
                        .Where(x => x.SpaceId == space.SpaceId)
                        .Select(x => x.PackageFamilyName)
                        .ToList();

                    var notificationCount = 0;
                    foreach (var packageId in packageIds)
                    {
                        notificationCount += dbConnection.Table<KRToastNotification>()
                            .Count(x => x.PackageFamilyName == packageId);
                    }

                    stats[space.SpaceId] = (appCount, notificationCount);
                }

                return stats;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new Dictionary<string, (int AppCount, int NotificationCount)>();
            }
        }

        #endregion

        #region Feedback Methods

        /// <summary>
        /// Submits user feedback to the database
        /// </summary>
        public bool SubmitFeedback(string title, string message, FeedbackCategory category, string email, string userId, string appVersion, string osVersion)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                var feedback = new KFeedback
                {
                    Title = title,
                    Message = message,
                    Category = category,
                    Email = email,
                    UserId = userId,
                    AppVersion = appVersion,
                    OSVersion = osVersion,
                    Status = FeedbackStatus.Submitted
                };

                dbConnection.InsertAll(new[] { feedback });
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets all feedback for a user
        /// </summary>
        public IList<KFeedback> GetUserFeedback(string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                return dbConnection.Table<KFeedback>()
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KFeedback>();
            }
        }

        /// <summary>
        /// Gets feedback by category for a user
        /// </summary>
        public IList<KFeedback> GetFeedbackByCategory(FeedbackCategory category, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                return dbConnection.Table<KFeedback>()
                    .Where(x => x.UserId == userId && x.Category == category)
                    .OrderByDescending(x => x.CreatedTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KFeedback>();
            }
        }

        /// <summary>
        /// Updates feedback status
        /// </summary>
        public bool UpdateFeedbackStatus(string feedbackId, FeedbackStatus status, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                var feedback = dbConnection.Table<KFeedback>()
                    .FirstOrDefault(x => x.Id == feedbackId && x.UserId == userId);

                if (feedback != null)
                {
                    feedback.Status = status;
                    feedback.UpdatedTime = DateTimeOffset.Now;
                    dbConnection.UpdateAll(new[] { feedback });
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        #endregion

        #region Sound Mapping Methods

        /// <summary>
        /// Gets all sound mappings for a user
        /// </summary>
        public IList<KSoundMapper> GetSoundMappings(string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                return dbConnection.Table<KSoundMapper>()
                    .Where(x => x.UserId == userId && x.IsEnabled)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new List<KSoundMapper>();
            }
        }

        /// <summary>
        /// Gets sound mapping for a specific package
        /// </summary>
        public NotificationSounds GetPackageSound(string packageFamilyName, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                var mapping = dbConnection.Table<KSoundMapper>()
                    .FirstOrDefault(x => x.PackageFamilyName == packageFamilyName &&
                                    x.UserId == userId &&
                                    x.IsEnabled);

                return mapping?.Sound ?? NotificationSounds.None;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return NotificationSounds.None;
            }
        }

        /// <summary>
        /// Adds or updates sound mapping for a package
        /// </summary>
        public bool AddOrUpdateSoundMapping(string packageFamilyName, NotificationSounds sound, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);

                // Check if mapping already exists
                var existingMapping = dbConnection.Table<KSoundMapper>()
                    .FirstOrDefault(x => x.PackageFamilyName == packageFamilyName && x.UserId == userId);

                if (existingMapping != null)
                {
                    // Update existing
                    existingMapping.Sound = sound;
                    existingMapping.UpdatedTime = DateTimeOffset.Now;
                    existingMapping.IsEnabled = sound != NotificationSounds.None; // Disable if None

                    dbConnection.UpdateAll(new[] { existingMapping });
                }
                else
                {
                    // Create new
                    var newMapping = new KSoundMapper
                    {
                        PackageFamilyName = packageFamilyName,
                        Sound = sound,
                        UserId = userId,
                        CreatedTime = DateTimeOffset.Now,
                        UpdatedTime = DateTimeOffset.Now,
                        IsEnabled = sound != NotificationSounds.None
                    };

                    dbConnection.InsertAll(new[] { newMapping });
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Removes sound mapping for a package (resets to None)
        /// </summary>
        public bool RemoveSoundMapping(string packageFamilyName, string userId)
        {
            try
            {
                return AddOrUpdateSoundMapping(packageFamilyName, NotificationSounds.None, userId);
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets packages grouped by their assigned sound
        /// </summary>
        public Dictionary<NotificationSounds, List<string>> GetPackagesBySound(string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(SmartNotificationConstants.CurrentUser);
                var mappings = dbConnection.Table<KSoundMapper>()
                    .Where(x => x.UserId == userId && x.IsEnabled && x.Sound != NotificationSounds.None)
                    .ToList();

                var result = new Dictionary<NotificationSounds, List<string>>();

                foreach (var mapping in mappings)
                {
                    if (!result.ContainsKey(mapping.Sound))
                    {
                        result[mapping.Sound] = new List<string>();
                    }
                    result[mapping.Sound].Add(mapping.PackageFamilyName);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return new Dictionary<NotificationSounds, List<string>>();
            }
        }

        #endregion

        #region Notification Management Methods

        /// <summary>
        /// Removes all notifications for a specific package
        /// </summary>
        public bool ClearPackageNotifications(string packageFamilyName, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(userId);

                var result = dbConnection.Execute(
                    $"DELETE FROM {nameof(KRToastNotification)} WHERE {nameof(KRToastNotification.PackageFamilyName)} = ?",
                    packageFamilyName);

                Logger.Info(LogManager.GetCallerInfo(), $"Cleared {result} notifications for package {packageFamilyName}");
                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets notification count for a specific package
        /// </summary>
        public int GetPackageNotificationCount(string packageFamilyName, string userId)
        {
            try
            {
                IDBConnection dbConnection = GetDBConnection(userId);

                return dbConnection.Table<KRToastNotification>()
                    .Count(x => x.PackageFamilyName == packageFamilyName);
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), ex.Message);
                return 0;
            }
        }

        #endregion






    }
}
