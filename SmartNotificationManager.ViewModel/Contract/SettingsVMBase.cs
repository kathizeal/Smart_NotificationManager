using SmartNotificationManger.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationManager.ViewModel.Contract
{
    public abstract class SettingsVMBase
    {
        public readonly ObservableCollection<KRPackageProfile> ExcludedPackages;

        public SettingsVMBase()
        {
            ExcludedPackages = new();
        }

        public abstract void GetExcludedPackages();
    }
}
