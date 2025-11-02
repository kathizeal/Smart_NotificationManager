using SQLite;
using WinCommon.Util;

namespace SmartNotificationManger
{
    public class KRPackageProfile : ObservableObject
    {
        private string _appDisplayName;
        private string _appDescription;

        [PrimaryKey]
        public string PackageFamilyName { get; set; }

        public string AppDisplayName
        {
            get => _appDisplayName;
            set => SetIfDifferent(ref _appDisplayName, value);
        }

        public string AppDescription
        {
            get => _appDescription;
            set => SetIfDifferent(ref _appDescription, value);
        }

        public string Publisher { get; set; }


        public KRPackageProfile() { }

        public KRPackageProfile DeepClone()
        {
            return new KRPackageProfile
            {
                PackageFamilyName = this.PackageFamilyName,
                AppDisplayName = this.AppDisplayName,
                AppDescription = this.AppDescription,
                Publisher = this.Publisher

            };
        }

        public void Update(KRPackageProfile newData)
        {
            if (newData == null) { return; }

            PackageFamilyName = newData.PackageFamilyName;
            AppDisplayName = newData.AppDisplayName;
            AppDescription = newData.AppDescription;
            Publisher = newData.Publisher;
        }
    }

}
