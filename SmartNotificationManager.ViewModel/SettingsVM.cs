using SmartNotificationLibrary.Domain;
using SmartNotificationManager.ViewModel.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationManager.ViewModel
{
    public class SettingsVM : SettingsVMBase
    {

        public override void GetExcludedPackages()
        {
             GetExcludedPackagesRequest request = new GetExcludedPackagesRequest(default);
            var getExcludedPackagesUseCase = new GetExcludedPackages(request, default);
            getExcludedPackagesUseCase.Execute();
        }





        #region PresenterCallback Class

        #endregion
    }
}
