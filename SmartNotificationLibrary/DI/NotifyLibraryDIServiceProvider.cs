using Microsoft.Extensions.DependencyInjection;
using SmartNotificationLibrary.DBHandler;
using SmartNotificationLibrary.DBHandler.Contract;
using WinCommon.DI;

namespace SmartNotificationLibrary.DI
{
    public class NotifyLibraryDIServiceProvider : DotNetDIServiceProviderBase
    {
        public static NotifyLibraryDIServiceProvider Instance { get { return LibraryDIServiceProviderSingleton.Instance; } }

        private NotifyLibraryDIServiceProvider()
        {

        }

        public void Initialize(IServiceCollection services)
        {
            services.AddSingleton<INotifyDBHandler, NotifyDBHandler>();
            //services.AddSingleton<IGetKToastsDataManager, GetKToastDataManager>();
            //services.AddSingleton<IUpdateKToastDataManager, UpdateKToastDataManager>();
            //services.AddSingleton<IGetAllSpaceDataManager, GetAllSpaceDataManager>();
            //services.AddSingleton<IGetAllKPackageProfilesDataManager, GetAllKPackageProfilesDataManager>();
            //services.AddSingleton<IAddPackageToSpaceDataManager, AddPackageToSpaceDataManager>();
            //services.AddSingleton<IGetPackageBySpaceDataManager, GetPackageBySpaceDataManager>();
            //services.AddSingleton<IAddAppsToConditionDataManager, AddAppsToConditionDataManager>();
            //services.AddSingleton<IGetNotificationsByConditionDataManager, GetNotificationsByConditionDataManager>();
            //services.AddSingleton<ISubmitFeedbackDataManager, SubmitFeedbackDataManager>();
            //services.AddSingleton<ISoundMappingDataManager, SoundMappingDataManager>();
            //services.AddSingleton<IClearPackageNotificationsDataManager, ClearPackageNotificationsDataManager>();
            //services.AddSingleton<IRemoveAppFromConditionDataManager, RemoveAppFromConditionDataManager>();

            BuildServiceProvider(services, true); // Build the service provider with default services
        }

        #region DIServiceProviderSingleton Class
        private class LibraryDIServiceProviderSingleton
        {
            // Explicit static constructor 
            static LibraryDIServiceProviderSingleton() { }

            //Marked as internal as it will be accessed from the enclosing class. It doesn't raise any problem, as the class itself is private.
            internal static readonly NotifyLibraryDIServiceProvider Instance = new NotifyLibraryDIServiceProvider();
        }
        #endregion
    }

}
