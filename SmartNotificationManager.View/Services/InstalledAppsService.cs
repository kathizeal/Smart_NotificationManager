using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using SmartNotificationManger.Entities;
using SmartNotificationManger.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace SmartNotificationManager.View.Services
{
    public class InstalledAppsService
    {
        // Win32 API imports for icon extraction
        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string lpIconPath, out ushort lpiIcon);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);


        /// <summary>
        /// Gets all installed applications (Win32 + UWP) excluding the current app
        /// </summary>
        public async Task<ObservableCollection<KRPackageProfile>> GetAllInstalledAppsAsync()
        {
            var apps = new ObservableCollection<KRPackageProfile>();

            // Current app identifiers to exclude
            const string currentAppName = SmartNotificationConstants.ApplicationName;
            const string currentAppPackageName = SmartNotificationConstants.AppPackageId;
            const string currentAppPublisher = "Kathi";

            // Get Win32 applications
            var win32Apps = await GetWin32AppsAsync();
            foreach (var app in win32Apps)
            {
                // Skip the current app based on display name and publisher
                if (IsCurrentApp(app, currentAppName, currentAppPackageName, currentAppPublisher))
                    continue;

                apps.Add(app);
            }

            // Get UWP applications
            var uwpApps = await GetUWPAppsAsync();
            foreach (var app in uwpApps)
            {
                // Skip the current app based on package name or display name
                if (IsCurrentApp(app, currentAppName, currentAppPackageName, currentAppPublisher))
                    continue;

                apps.Add(app);
            }

            return apps;
        }

        /// <summary>
        /// Determines if the given app is the current INotify app
        /// </summary>
        private bool IsCurrentApp(KRPackageProfile app, string currentAppName, string currentAppPackageName, string currentAppPublisher)
        {
            // Check by package name (for UWP apps)
            if (!string.IsNullOrEmpty(app.PackageFamilyName) &&
                app.PackageFamilyName.Equals(currentAppPackageName, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check by display name
            if (!string.IsNullOrEmpty(app.AppDisplayName) &&
                app.AppDisplayName.Equals(currentAppName, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check by display name and publisher combination
            if (!string.IsNullOrEmpty(app.AppDisplayName) &&
                !string.IsNullOrEmpty(app.Publisher) &&
                app.AppDisplayName.Equals(currentAppName, StringComparison.OrdinalIgnoreCase) &&
                app.Publisher.Equals(currentAppPublisher, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        /// <summary>
        /// Gets Win32 applications from registry
        /// </summary>
        public async Task<List<KRPackageProfile>> GetWin32AppsAsync()
        {
            return await Task.Run(() =>
            {
                var apps = new List<KRPackageProfile>();
                var registryKeys = new[]
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                };

                foreach (var keyPath in registryKeys)
                {
                    try
                    {
                        using (var key = Registry.LocalMachine.OpenSubKey(keyPath))
                        {
                            if (key != null)
                            {
                                foreach (var subKeyName in key.GetSubKeyNames())
                                {
                                    using (var subKey = key.OpenSubKey(subKeyName))
                                    {
                                        if (subKey != null)
                                        {
                                            KRPackageProfile app = CreateWin32AppInfo(subKey, subKeyName);
                                            if (app != null && !string.IsNullOrEmpty(app.AppDisplayName) && !AppToIgnoreWin32(app))
                                            {
                                                apps.Add(app);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error reading registry: {ex.Message}");
                    }
                }

                return apps.OrderBy(a => a.AppDisplayName).ToList();
            });
        }

        private bool AppToIgnoreWin32(KRPackageProfile packageProfile)
        {
            // Ignore if Publisher contains "Microsoft Corporation"
            if (!string.IsNullOrEmpty(packageProfile.Publisher) &&
                packageProfile.Publisher.Contains(value: "Microsoft Corporation", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Future: Add more ignore rules here (e.g., check against a list of ignored apps)
            // Example:
            // var ignoreList = new List<string> { "SomeApp", "AnotherApp" };
            // if (ignoreList.Any(x => packageProfile.Publisher.Contains(x, StringComparison.OrdinalIgnoreCase)))
            //     return true;

            return false;
        }

        /// <summary>
        /// Gets UWP applications using PackageManager
        /// </summary>
        /// <summary>
        /// Gets UWP applications using PackageManager
        /// </summary>
        public async Task<List<KRPackageProfile>> GetUWPAppsAsync()
        {
            return await Task.Run(() =>
            {
                var apps = new List<KRPackageProfile>();

                try
                {
                    var packageManager = new PackageManager();
                    var packages = packageManager.FindPackagesForUser("");

                    foreach (var package in packages)
                    {
                        try
                        {
                            // Skip only frameworks and packages without display names
                            // Allow system packages that are user-facing apps like Clock, Calculator, etc.
                            if (package.IsFramework ||
                                string.IsNullOrEmpty(package.DisplayName))
                                continue;

                            // Additional filtering for truly internal system packages
                            // Keep packages that users typically interact with
                            if (package.SignatureKind == PackageSignatureKind.System)
                            {
                                // Use your existing allowlist for system apps
                                // Allow specific system apps that users commonly use
                                var allowedSystemApps = new[]
                                {
    // Core Windows Apps
    "Microsoft.WindowsAlarms", // Clock app
    "Microsoft.WindowsCalculator", // Calculator
    "Microsoft.WindowsCamera", // Camera
    "Microsoft.WindowsMaps", // Maps
    "Microsoft.WindowsNotepad", // Notepad
    "Microsoft.Paint", // Paint
    "Microsoft.WindowsTerminal", // Terminal
    "Microsoft.WindowsSoundRecorder", // Sound Recorder
    "Microsoft.ScreenSketch", // Snipping Tool
    "Microsoft.MicrosoftStickyNotes", // Sticky Notes
    "Microsoft.WindowsFeedbackHub", // Feedback Hub
    "Microsoft.GetHelp", // Get Help
    "Microsoft.Getstarted", // Tips
    "Microsoft.Microsoft3DViewer", // 3D Viewer
    "Microsoft.MSPaint", // Paint 3D
    "Microsoft.Office.OneNote", // OneNote
    "Microsoft.People", // People
    "Microsoft.Windows.Photos", // Photos
    "Microsoft.WindowsStore", // Microsoft Store
    
    // Xbox and Gaming
    "Microsoft.Xbox.TCUI", // Xbox
    "Microsoft.XboxApp", // Xbox Console Companion
    "Microsoft.XboxGameOverlay", // Xbox Game Bar
    "Microsoft.XboxGamingOverlay", // Xbox Gaming Overlay
    "Microsoft.XboxIdentityProvider", // Xbox Identity Provider
    "Microsoft.XboxSpeechToTextOverlay", // Xbox Speech to Text
    "Microsoft.GamingApp", // Xbox (New)
    "Microsoft.XboxGameCallableUI", // Xbox Game UI
    
    // Communication & Social
    "Microsoft.YourPhone", // Phone Link
    "Microsoft.People", // People
    "Microsoft.Messaging", // Messaging
    "Microsoft.CommsPhone", // Phone
    "Microsoft.SkypeApp", // Skype
    
    // Media & Entertainment
    "Microsoft.ZuneMusic", // Groove Music
    "Microsoft.ZuneVideo", // Movies & TV
    "Microsoft.WindowsMediaPlayer", // Windows Media Player
    "SpotifyAB.SpotifyMusic", // Spotify
    "Netflix.Netflix", // Netflix
    "5319275A.WhatsAppDesktop", // WhatsApp
    "TelegramMessengerLLP.TelegramDesktop", // Telegram
    "Discord.Discord", // Discord
    "ZoomVideoCommunications.ZoomRooms", // Zoom
    "Microsoft.Teams", // Microsoft Teams
    
    // Productivity & Office
    "Microsoft.Office.Desktop", // Office Desktop Apps
    "Microsoft.OutlookForWindows", // New Outlook
    "Microsoft.MicrosoftOfficeHub", // Office Hub
    "Microsoft.Office.Word", // Word
    "Microsoft.Office.Excel", // Excel
    "Microsoft.Office.PowerPoint", // PowerPoint
    "Microsoft.Todos", // Microsoft To Do
    "Microsoft.PowerAutomateDesktop", // Power Automate
    "Microsoft.PowerBI", // Power BI
    "9NBLGGH4NNS1", // Adobe Photoshop Elements
    "AdobeSystemsIncorporated.AdobePhotoshopLightroom", // Adobe Lightroom
    
    // Web Browsers
    "Microsoft.MicrosoftEdge", // Microsoft Edge
    "Mozilla.Firefox", // Firefox
    "Google.Chrome", // Google Chrome
    "Opera.Opera", // Opera
    
    // Development Tools
    "Microsoft.VisualStudioCode", // Visual Studio Code
    "GitHubDesktop.GitHubDesktop", // GitHub Desktop
    "Microsoft.WindowsSubsystemForLinux", // WSL
    "Microsoft.PowerShell", // PowerShell
    
    // News & Information
    "Microsoft.BingWeather", // Weather
    "Microsoft.BingNews", // News
    "Microsoft.BingFinance", // Money
    "Microsoft.BingSports", // Sports
    "Microsoft.BingTravel", // Travel
    "Microsoft.BingHealthAndFitness", // Health & Fitness
    "Microsoft.BingFoodAndDrink", // Food & Drink
    "MSTeams", // Microsoft Teams (alternative)
    
    // Utilities & Tools
    "Microsoft.WindowsReadingList", // Reading List
    "Microsoft.MixedReality.Portal", // Mixed Reality Portal
    "Microsoft.Windows.Cortana", // Cortana
    "Microsoft.WindowsBackup", // Windows Backup
    "Microsoft.RemoteDesktop", // Remote Desktop
    "Microsoft.Windows.SecHealthUI", // Windows Security
    "WinZip.WinZip", // WinZip
    "RARLab.WinRAR", // WinRAR
    "9NBLGGH4Z1JC", // VLC Media Player
    "VideoLAN.VLC", // VLC (alternative)
    
    // Popular Store Apps
    "Amazon.com.Amazon", // Amazon
    "TheNewYorkTimes.NYTimes", // NY Times
    "Facebook.Facebook", // Facebook
    "Instagram.Instagram", // Instagram
    "Twitter.Twitter", // Twitter
    "LinkedInCorporation.LinkedIn", // LinkedIn
    "Uber.Uber", // Uber
    "Pinterest.Pinterest", // Pinterest
    "TikTok.TikTok", // TikTok
    "Flipboard.Flipboard", // Flipboard
    
    // Gaming Platforms
    "ValveSoftware.Steam", // Steam
    "EpicGames.EpicGamesLauncher", // Epic Games
    "Ubisoft.UbisoftConnect", // Ubisoft Connect
    "ElectronicArts.EADesktop", // EA Desktop
    "Blizzard.BattleNet", // Battle.net
    
    // Creative & Design
    "Adobe.CC.XD", // Adobe XD
    "Adobe.Illustrator", // Adobe Illustrator
    "Canva.Canva", // Canva
    "GIMP.GIMP", // GIMP
    "Inkscape.Inkscape", // Inkscape
    
    // File Management
    "Microsoft.OneDrive", // OneDrive
    "Dropbox.Dropbox", // Dropbox
    "Google.GoogleDrive", // Google Drive
    "Box.Box", // Box
    
    // Education & Learning
    "KhanAcademy.KhanAcademy", // Khan Academy
    "Duolingo.Duolingo", // Duolingo
    "Coursera.Coursera", // Coursera
    "Microsoft.Whiteboard", // Microsoft Whiteboard
    
    // Finance & Banking
    "PayPal.PayPal", // PayPal
    "Microsoft.BingFinance", // Microsoft Money
    "Mint.Mint", // Mint
    
    // Health & Fitness
    "MyFitnessPal.MyFitnessPal", // MyFitnessPal
    "Fitbit.Fitbit", // Fitbit
    "Nike.NikeTrainingClub", // Nike Training Club
    
    // Travel & Navigation
    "Uber.Uber", // Uber
    "Airbnb.Airbnb", // Airbnb
    "Booking.Booking", // Booking.com
    "TripAdvisor.TripAdvisor", // TripAdvisor
    
    // Shopping
    "Amazon.com.Amazon", // Amazon
    "eBay.eBay", // eBay
    "Walmart.Walmart", // Walmart
    "Target.Target", // Target
    
    // System & Windows Features
    "Microsoft.Windows.CloudExperienceHost", // Cloud Experience (if user-facing)
    "Microsoft.AccountsControl", // Accounts Control (if user-facing)
};

                                if (!allowedSystemApps.Contains(package.Id.Name))
                                    continue;
                            }
                            else
                            {
                                // For non-system packages (Store apps), exclude only problematic ones
                                var excludedApps = new[]
                                {
        // Add any specific Store apps you want to exclude
        "Microsoft.Advertising.Xaml", // Internal advertising framework
        "Microsoft.NET.Native.Framework", // .NET Native framework
        "Microsoft.NET.Native.Runtime", // .NET Native runtime
    };

                                if (excludedApps.Contains(package.Id.Name))
                                    continue;
                            }

                            var app = new KRPackageProfile
                            {
                                AppDisplayName = package.DisplayName,
                                Publisher = package.PublisherDisplayName,
                                PackageFamilyName = package.Id.FamilyName
                            };

                            apps.Add(app);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing package {package.Id.Name}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting UWP apps: {ex.Message}");
                }

                return apps.OrderBy(a => a.AppDisplayName).ToList();
            });
        }
        /// <summary>
        /// Gets applications with icons loaded, excluding the current app
        /// </summary>
        public async Task<ObservableCollection<KRPackageProfile>> GetInstalledAppsWithIconsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Starting GetInstalledAppsWithIconsAsync...");

                var apps = await GetAllInstalledAppsAsync(); // This now already excludes the current app
                System.Diagnostics.Debug.WriteLine($"Retrieved {apps.Count} total apps (excluding current app)");



                return apps;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetInstalledAppsWithIconsAsync: {ex.Message}");
                return new ObservableCollection<KRPackageProfile>();
            }
        }

        private KRPackageProfile CreateWin32AppInfo(RegistryKey key, string subKeyName)
        {
            try
            {
                var displayName = key.GetValue("DisplayName")?.ToString();
                var systemComponent = key.GetValue("SystemComponent");
                var parentKeyName = key.GetValue("ParentKeyName");
                var windowsInstaller = key.GetValue("WindowsInstaller");

                // Skip system components and updates
                if (string.IsNullOrEmpty(displayName) ||
                    (systemComponent != null && systemComponent.ToString() == "1") ||
                    !string.IsNullOrEmpty(parentKeyName?.ToString()) ||
                    displayName.Contains("Update for") ||
                    displayName.Contains("Hotfix for"))
                {
                    return null;
                }

                var app = new KRPackageProfile
                {
                    AppDisplayName = displayName,
                    Publisher = key.GetValue("Publisher")?.ToString() ?? "",
                };



                app.PackageFamilyName = app.AppDisplayName + "_" + app.Publisher;
                app.PackageFamilyName = app.PackageFamilyName.Trim().Replace(" ", "_");

                return app;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating app info: {ex.Message}");
                return null;
            }
        }

    }

}
