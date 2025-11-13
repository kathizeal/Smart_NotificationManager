using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using SmartNotificationLibrary.DBHandler.Contract;
using SmartNotificationLibrary.DI;
using SmartNotificationLibrary.Enums;
using SmartNotificationLibrary.Services;
using SmartNotificationManger;
using SmartNotificationManger.Entities;
using SmartNotificationManger.Entities.Constants;
using SmartNotificationManger.Entities.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace SmartNotificationManager.View.Services
{
    /// <summary>
    /// Service for creating and managing toast notifications from INotify app
    /// Supports priority and space categorization with modern AppNotificationBuilder API
    /// </summary>
    public class INotifyToastService : IDisposable
    {
        private static readonly Lazy<INotifyToastService> _instance = new(() => new INotifyToastService());
        public static INotifyToastService Instance => _instance.Value;

        private readonly NotificationFilterCacheService _cacheService;
        private readonly INotifyDBHandler _dbHandler;
        private readonly string _appDisplayName = SmartNotificationConstants.ApplicationName;
        private readonly string _appId;
        private bool _disposed = false;

        private INotifyToastService()
        {
            try
            {
                _cacheService = NotificationFilterCacheService.Instance;
                _dbHandler = NotifyLibraryDIServiceProvider.Instance.GetService<INotifyDBHandler>();
                _appId = Package.Current.Id.FamilyName;
                Debug.WriteLine("INotifyToastService initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing INotifyToastService: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a received notification and creates INotify toast if categorized
        /// </summary>
        public async Task ProcessNotificationAsync(KRToastBObj originalNotification)
        {
            try
            {
                if (originalNotification?.ToastPackageProfile == null || originalNotification.NotificationData == null)
                    return;

                var packageFamilyName = originalNotification.ToastPackageProfile.PackageFamilyName;
                var appDisplayName = originalNotification.ToastPackageProfile.AppDisplayName;

                // Check if app has priority assignment
                var priorityInfo = await GetAppPriorityInfoAsync(packageFamilyName);

                // Check if app belongs to any spaces
                var spaceInfo = await GetAppSpaceInfoAsync(packageFamilyName);

                // Only create toast if app is categorized
                if (priorityInfo.HasPriority || spaceInfo.HasSpaces)
                {
                    await CreateCategorizedToastAsync(originalNotification, priorityInfo, spaceInfo);
                }
                else
                {
                    Debug.WriteLine($"App {appDisplayName} not categorized, skipping toast creation");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing notification for toast: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets priority information for an app
        /// </summary>
        private async Task<PriorityInfo> GetAppPriorityInfoAsync(string packageFamilyName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    foreach (Priority priority in Enum.GetValues<Priority>())
                    {
                        if (priority == Priority.None) continue;

                        if (_cacheService.IsPackageInPriorityCategory(packageFamilyName, priority.ToString()))
                        {
                            return new PriorityInfo
                            {
                                HasPriority = true,
                                Priority = priority,
                                PriorityText = GetPriorityDisplayText(priority),
                                PriorityIcon = GetPriorityIcon(priority)
                            };
                        }
                    }

                    return new PriorityInfo { HasPriority = false };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting priority info: {ex.Message}");
                    return new PriorityInfo { HasPriority = false };
                }
            });
        }

        /// <summary>
        /// Gets space information for an app
        /// </summary>
        private async Task<SpaceInfo> GetAppSpaceInfoAsync(string packageFamilyName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var spaces = new List<string>();
                    var spaceIds = new[] { "Space1", "Space2", "Space3" };

                    foreach (var spaceId in spaceIds)
                    {
                        if (_cacheService.IsPackageInSpace(packageFamilyName, spaceId))
                        {
                            spaces.Add(GetSpaceDisplayText(spaceId));
                        }
                    }

                    return new SpaceInfo
                    {
                        HasSpaces = spaces.Count > 0,
                        Spaces = spaces,
                        SpacesText = spaces.Count > 0 ? string.Join(", ", spaces) : "",
                        SpaceIcon = spaces.Count > 0 ? GetSpaceIcon() : ""
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting space info: {ex.Message}");
                    return new SpaceInfo { HasSpaces = false };
                }
            });
        }

        /// <summary>
        /// Creates and shows a categorized toast notification using modern AppNotificationBuilder
        /// </summary>
        private async Task CreateCategorizedToastAsync(KRToastBObj originalNotification, PriorityInfo priorityInfo, SpaceInfo spaceInfo)
        {
            try
            {
                // Build the enhanced title and content
                var enhancedTitle = BuildEnhancedTitle(originalNotification, priorityInfo, spaceInfo);
                var enhancedContent = BuildEnhancedContent(originalNotification, priorityInfo, spaceInfo);
                //var categoryTags = BuildCategoryTags(priorityInfo, spaceInfo);

                // Get appropriate sound
                var notificationSound = GetNotificationSound(priorityInfo, spaceInfo, originalNotification.ToastPackageProfile.PackageFamilyName);

                Debug.WriteLine($"🔍 Creating modern toast for {originalNotification.ToastPackageProfile.AppDisplayName}");
                Debug.WriteLine($"🔍 Title: {enhancedTitle}");
                Debug.WriteLine($"🔍 Content: {enhancedContent}");
                Debug.WriteLine($"🔊 Sound: {notificationSound} ({NotificationSoundHelper.GetSoundTypeDescription(notificationSound)})");
                //Debug.WriteLine($"🔍 Tags: {categoryTags}");

                // Create notification using modern AppNotificationBuilder
                var builder = new AppNotificationBuilder()
                    .AddText(enhancedTitle)
                    .AddText(enhancedContent);
                //.AddText(categoryTags);

                // Apply sound based on type
                ApplyNotificationSound(builder, notificationSound);

                // Add action buttons
                builder.AddButton(new AppNotificationButton("View in INotify")
                    .AddArgument("action", "view")
                    .AddArgument("notificationId", originalNotification.NotificationData.NotificationId)
                    .AddArgument("originalAppName", originalNotification.ToastPackageProfile.AppDisplayName)
                    .AddArgument("originalPackage", originalNotification.ToastPackageProfile.PackageFamilyName));

                builder.AddButton(new AppNotificationButton("Dismiss")
                    .AddArgument("action", "dismiss")
                    .AddArgument("notificationId", originalNotification.NotificationData.NotificationId));

                // Add custom data for activation handling
                if (priorityInfo.HasPriority)
                {
                    builder.AddArgument("priority", priorityInfo.Priority.ToString());
                }
                if (spaceInfo.HasSpaces)
                {
                    builder.AddArgument("spaces", spaceInfo.SpacesText);
                }

                // Build the notification
                var notification = builder.BuildNotification();

                // Set expiration time (1 hour from now)
                notification.Expiration = DateTimeOffset.Now.AddHours(1);

                // Show the notification using modern API
                AppNotificationManager.Default.Show(notification);

                Debug.WriteLine($"✅ Created modern categorized toast for {originalNotification.ToastPackageProfile.AppDisplayName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error creating categorized toast: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");

            }
        }


        /// <summary>
        /// Creates a test toast with specific sound for testing sound system
        /// </summary>
        private async Task CreateSoundTestToast(string testName, NotificationSounds sound)
        {
            try
            {
                var builder = new AppNotificationBuilder()
                    .AddText($"🔔 {testName}")
                    .AddText($"Testing sound: {NotificationSoundHelper.GetSoundDisplayText(sound)}")
                    .AddText($"Sound type: {NotificationSoundHelper.GetSoundTypeDescription(sound)}");

                // Apply the specific sound
                ApplyNotificationSound(builder, sound);

                var notification = builder.BuildNotification();
                AppNotificationManager.Default.Show(notification);

                Debug.WriteLine($"✅ Created sound test toast: {testName} with {sound}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error creating sound test toast: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds enhanced title with category information
        /// </summary>
        private string BuildEnhancedTitle(KRToastBObj notification, PriorityInfo priorityInfo, SpaceInfo spaceInfo)
        {
            var parts = new List<string>();

            if (priorityInfo.HasPriority)
            {
                parts.Add($"{priorityInfo.PriorityIcon} {priorityInfo.PriorityText}");
            }

            if (spaceInfo.HasSpaces)
            {
                parts.Add($"{spaceInfo.SpaceIcon} {spaceInfo.SpacesText}");
            }

            var categorization = parts.Count > 0 ? $"[{string.Join(" | ", parts)}]" : "";
            var appName = notification.ToastPackageProfile.AppDisplayName;

            return $"{categorization} {appName}";
        }

        /// <summary>
        /// Builds enhanced content with original notification content
        /// </summary>
        private string BuildEnhancedContent(KRToastBObj notification, PriorityInfo priorityInfo, SpaceInfo spaceInfo)
        {
            var content = notification.NotificationData.NotificationMessage ?? "";
            return $"{content}";
        }

        /// <summary>
        /// Builds category tags for display
        /// </summary>
        private string BuildCategoryTags(PriorityInfo priorityInfo, SpaceInfo spaceInfo)
        {
            var tags = new List<string>();

            if (priorityInfo.HasPriority)
            {
                tags.Add($"Priority: {priorityInfo.PriorityText}");
            }

            if (spaceInfo.HasSpaces)
            {
                tags.Add($"Spaces: {spaceInfo.SpacesText}");
            }

            return tags.Count > 0 ? string.Join(" • ", tags) : "Managed by INotify";
        }



        /// <summary>
        /// Gets legacy sound path for XML-based toast notifications
        /// </summary>
        private string GetLegacySoundPath(NotificationSounds sound)
        {
            try
            {
                if (sound == NotificationSounds.None)
                {
                    return "ms-winsoundevent:Notification.Default";
                }

                if (NotificationSoundHelper.IsCustomSound(sound))
                {
                    return NotificationSoundHelper.GetCustomSoundPath(sound);
                }

                if (NotificationSoundHelper.IsSystemSound(sound))
                {
                    // For system sounds in legacy mode, map to equivalent system sound URIs
                    // New approach - Cross-platform
                    var soundUri = NotificationSoundHelper.GetSoundUri(sound);
                    return soundUri;
                    //builder.SetAudioUri(new Uri(soundUri)); return $"ms-winsoundevent:Notification.{systemSound}";
                }

                return "ms-winsoundevent:Notification.Default";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting legacy sound path for {sound}: {ex.Message}");
                return "ms-winsoundevent:Notification.Default";
            }
        }

        /// <summary>
        /// Applies notification sound to AppNotificationBuilder based on sound type
        /// Uses SetAudioUri for all sounds (both custom and system) with URI strings
        /// </summary>
        private void ApplyNotificationSound(AppNotificationBuilder builder, NotificationSounds sound)
        {
            try
            {
                if (sound == NotificationSounds.None)
                {
                    // Use default system sound - use URI instead of enum
                    builder.SetAudioUri(new Uri("ms-winsoundevent:Notification.Default"));
                    Debug.WriteLine($"🔊 Applied default system sound");
                    return;
                }

                if (NotificationSoundHelper.IsCustomSound(sound))
                {
                    // Custom sound - use SetAudioUri with ms-appx URI
                    var soundPath = NotificationSoundHelper.GetCustomSoundPath(sound);
                    try
                    {
                        builder.SetAudioUri(new Uri(soundPath));
                        Debug.WriteLine($"🔊 Applied custom sound: {sound} -> {soundPath}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"⚠️ Failed to set custom sound {sound}: {ex.Message}, falling back to default");
                        builder.SetAudioUri(new Uri("ms-winsoundevent:Notification.Default"));
                    }
                }
                else if (NotificationSoundHelper.IsSystemSound(sound))
                {
                    // System sound - use SetAudioUri with ms-winsoundevent URI
                    var soundUri = NotificationSoundHelper.GetSystemSoundEventUri(sound);
                    builder.SetAudioUri(new Uri(soundUri));
                    Debug.WriteLine($"🔊 Applied system sound: {sound} -> {soundUri}");
                }
                else
                {
                    // Fallback to default
                    builder.SetAudioUri(new Uri("ms-winsoundevent:Notification.Default"));
                    Debug.WriteLine($"🔊 Unknown sound type {sound}, using default");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error applying notification sound {sound}: {ex.Message}");
                // Fallback to default sound
                try
                {
                    builder.SetAudioUri(new Uri("ms-winsoundevent:Notification.Default"));
                }
                catch
                {
                    // If even default fails, continue without sound
                    Debug.WriteLine($"⚠️ Failed to apply even default sound");
                }
            }
        }

        /// <summary>
        /// Gets the appropriate notification sound based on categorization and sound mappings
        /// Now supports both custom sounds (SetAudioUri) and system sounds (SetAudioEvent)
        /// </summary>
        private NotificationSounds GetNotificationSound(PriorityInfo priorityInfo, SpaceInfo spaceInfo, string packageFamilyName)
        {
            try
            {
                // Get custom sound mapping for this package
                var customSound = _dbHandler?.GetPackageSound(packageFamilyName, SmartNotificationConstants.CurrentUser) ?? NotificationSounds.None;

                Debug.WriteLine($"🔊 Sound mapping for {packageFamilyName}: {customSound} ({NotificationSoundHelper.GetSoundTypeDescription(customSound)})");

                return customSound;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting notification sound: {ex.Message}");
                return NotificationSounds.None;
            }
        }

        #region Helper Methods

        private string GetPriorityDisplayText(Priority priority) => priority switch
        {
            Priority.High => "High Priority",
            Priority.Medium => "Medium Priority",
            Priority.Low => "Low Priority",
            _ => "Priority"
        };

        private string GetPriorityIcon(Priority priority) => priority switch
        {
            Priority.High => "🔴",
            Priority.Medium => "🟡",
            Priority.Low => "🟢",
            _ => "📌"
        };

        private string GetSpaceDisplayText(string spaceId) => spaceId switch
        {
            "Space1" => "Space 1",
            "Space2" => "Space 2",
            "Space3" => "Space 3",
            _ => spaceId
        };

        private string GetSpaceIcon() => "🏷️";

        #endregion

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
