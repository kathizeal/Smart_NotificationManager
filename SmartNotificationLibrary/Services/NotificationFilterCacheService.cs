using SmartNotificationLibrary.DBHandler.Contract;
using SmartNotificationLibrary.DI;
using SmartNotificationLibrary.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WinLogger;
using WinLogger.Contract;

namespace SmartNotificationLibrary.Services
{

    /// <summary>
    /// Cache service for notification filtering to improve real-time notification processing performance
    /// </summary>
    public class NotificationFilterCacheService : IDisposable
    {
        private readonly ILogger Logger = LogManager.GetLogger();
        private static readonly Lazy<NotificationFilterCacheService> _instance = new(() => new NotificationFilterCacheService());
        public static NotificationFilterCacheService Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, Priority> _packagePriorityCache = new();
        private readonly ConcurrentDictionary<string, HashSet<string>> _spacePackageCache = new();
        private readonly object _cacheUpdateLock = new object();
        private DateTimeOffset _lastCacheUpdate = DateTimeOffset.MinValue;
        private readonly TimeSpan _cacheValidityDuration = TimeSpan.FromMinutes(5); // Cache for 5 minutes

        private INotifyDBHandler _dbHandler;

        private NotificationFilterCacheService()
        {
            try
            {
                // Initialize DB handler through DI
                _dbHandler = NotifyLibraryDIServiceProvider.Instance.GetService<INotifyDBHandler>();
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error initializing NotificationFilterCacheService: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a package belongs to a specific priority category
        /// </summary>
        public bool IsPackageInPriorityCategory(string packageFamilyName, string priorityLevel)
        {
            try
            {
                if (string.IsNullOrEmpty(packageFamilyName) || string.IsNullOrEmpty(priorityLevel))
                    return false;

                // Parse priority level
                if (!Enum.TryParse<Priority>(priorityLevel, true, out var targetPriority))
                    return false;

                // Ensure cache is up to date
                EnsurePriorityCacheUpdated();

                // Check cache
                if (_packagePriorityCache.TryGetValue(packageFamilyName, out var packagePriority))
                {
                    return packagePriority == targetPriority;
                }

                // If not in cache, package doesn't have a custom priority, so it doesn't belong to any specific category
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(LogManager.GetCallerInfo(), $"Error checking package priority: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a package belongs to a specific space
        /// </summary>
        public bool IsPackageInSpace(string packageFamilyName, string spaceId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageFamilyName) || string.IsNullOrEmpty(spaceId))
                    return false;

                // Ensure cache is up to date
                EnsureSpaceCacheUpdated();

                // Check cache
                if (_spacePackageCache.TryGetValue(spaceId, out var packagesInSpace))
                {
                    return packagesInSpace.Contains(packageFamilyName);
                }

                return false;
            }
            catch (Exception ex)
            {

                Logger.Error(LogManager.GetCallerInfo(), $"Error checking package space membership: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Invalidates the cache to force a refresh on next access
        /// </summary>
        public void InvalidateCache()
        {
            lock (_cacheUpdateLock)
            {
                _packagePriorityCache.Clear();
                _spacePackageCache.Clear();
                _lastCacheUpdate = DateTimeOffset.MinValue;

                Logger.Info(LogManager.GetCallerInfo(), "NotificationFilterCacheService cache invalidated");
            }
        }

        /// <summary>
        /// Ensures the priority cache is up to date
        /// </summary>
        private void EnsurePriorityCacheUpdated()
        {
            lock (_cacheUpdateLock)
            {
                if (IsCacheValid() || _dbHandler == null)
                    return;

                try
                {
                    // Load custom priority apps from database
                    var priorityApps = _dbHandler.GetCustomPriorityApps(INotifyConstant.CurrentUser);

                    // Clear existing cache
                    _packagePriorityCache.Clear();

                    // Populate cache
                    foreach (var app in priorityApps)
                    {
                        if (!string.IsNullOrEmpty(app.PackageName))
                        {
                            _packagePriorityCache.TryAdd(app.PackageName, app.Priority);
                        }
                    }

                    Logger.Info(LogManager.GetCallerInfo(), $"Updated priority cache with {priorityApps.Count} entries");
                }
                catch (Exception ex)
                {
                    Logger.Error(LogManager.GetCallerInfo(), $"Error updating priority cache: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Ensures the space cache is up to date
        /// </summary>
        private void EnsureSpaceCacheUpdated()
        {
            lock (_cacheUpdateLock)
            {
                if (IsCacheValid() || _dbHandler == null)
                    return;

                try
                {
                    // Load all spaces from database
                    var spaces = _dbHandler.GetAllSpaceMappers(INotifyConstant.CurrentUser);

                    // Clear existing cache
                    _spacePackageCache.Clear();

                    // Populate cache for each space
                    foreach (var space in spaces)
                    {
                        var packagesInSpace = _dbHandler.GetPackagesBySpaceId(space.SpaceId, INotifyConstant.CurrentUser);
                        var packageFamilyNames = new HashSet<string>(
                            packagesInSpace.Where(p => !string.IsNullOrEmpty(p.PackageFamilyName))
                                          .Select(p => p.PackageFamilyName)
                        );

                        _spacePackageCache.TryAdd(space.SpaceId, packageFamilyNames);
                    }

                    _lastCacheUpdate = DateTimeOffset.Now;
                    Logger.Info(LogManager.GetCallerInfo(), $"Updated space cache with {spaces.Count} spaces");
                }
                catch (Exception ex)
                {
                    Logger.Error(LogManager.GetCallerInfo(), $"Error updating space cache: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Checks if the cache is still valid based on the last update time
        /// </summary>
        private bool IsCacheValid()
        {
            return DateTimeOffset.Now - _lastCacheUpdate < _cacheValidityDuration;
        }

        /// <summary>
        /// Adds or updates a package priority in the cache
        /// </summary>
        public void UpdatePackagePriorityInCache(string packageName, Priority priority)
        {
            if (!string.IsNullOrEmpty(packageName))
            {
                _packagePriorityCache.AddOrUpdate(packageName, priority, (key, oldValue) => priority);
                Logger.Info(LogManager.GetCallerInfo(), $"Updated priority cache for package {packageName}: {priority}");
            }
        }

        /// <summary>
        /// Adds a package to a space in the cache
        /// </summary>
        public void AddPackageToSpaceInCache(string packageFamilyName, string spaceId)
        {
            if (!string.IsNullOrEmpty(packageFamilyName) && !string.IsNullOrEmpty(spaceId))
            {
                _spacePackageCache.AddOrUpdate(spaceId,
                    new HashSet<string> { packageFamilyName },
                    (key, existingSet) =>
                    {
                        existingSet.Add(packageFamilyName);
                        return existingSet;
                    });

                Logger.Info(LogManager.GetCallerInfo(), $"Added package {packageFamilyName} to space {spaceId} in cache");
            }
        }

        /// <summary>
        /// Removes a package from a space in the cache
        /// </summary>
        public void RemovePackageFromSpaceInCache(string packageFamilyName, string spaceId)
        {
            if (!string.IsNullOrEmpty(packageFamilyName) && !string.IsNullOrEmpty(spaceId))
            {
                if (_spacePackageCache.TryGetValue(spaceId, out var packagesInSpace))
                {
                    packagesInSpace.Remove(packageFamilyName);
                    Logger.Info(LogManager.GetCallerInfo(), $"Removed package {packageFamilyName} from space {spaceId} in cache");
                }
            }
        }

        public void Dispose()
        {
            _packagePriorityCache.Clear();
            _spacePackageCache.Clear();
        }
    }
}
