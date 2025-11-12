using SmartNotificationLibrary.DI;
using SmartNotificationManger.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinCommon.Error;
using WinCommon.Util;

namespace SmartNotificationLibrary.Domain
{
    public interface IGetExcludedPackagesDataManager
    {
        Task GetExcludedPackages(GetExcludedPackagesRequest request, ICallback<GetExcludedPackagesResponse> callback);
    }

    public sealed class GetExcludedPackagesRequest
    {
        public readonly string UserId;

        public GetExcludedPackagesRequest(string userId)
        {
            UserId = userId;
        }
    }

    public sealed class GetExcludedPackagesResponse
    {
        public readonly IList<KRPackageProfile> ExcludedPackages;

        public GetExcludedPackagesResponse(IList<KRPackageProfile> excludedPackages)
        {
            ExcludedPackages = excludedPackages;
        }
    }

    public interface IGetExcludedPackagesCallback : ICallback<GetExcludedPackagesResponse>
    {
    }

    public sealed class GetExcludedPackages : UseCaseBase<GetExcludedPackagesResponse>
    {
        private readonly GetExcludedPackagesRequest _request;
        private readonly IGetExcludedPackagesDataManager _dataManager;

        public GetExcludedPackages(GetExcludedPackagesRequest request, IGetExcludedPackagesCallback callback) : base(callback)
        {
            _request = request;
            _dataManager = NotifyLibraryDIServiceProvider.Instance.GetService<IGetExcludedPackagesDataManager>();
        }

        protected override async void Action()
        {
            _= _dataManager.GetExcludedPackages(_request, callback: new UsecaseCallback(this));
        }

        class UsecaseCallback : CallbackBase<GetExcludedPackagesResponse>
        {
            private GetExcludedPackages _useCase { get; set; }

            public UsecaseCallback(GetExcludedPackages useCase)
            {
                _useCase = useCase;
            }

            public override void OnSuccess(ZResponse<GetExcludedPackagesResponse> response)
            {
                _useCase.PresenterCallback?.OnSuccess(response);
            }

            public override void OnError(ZError error)
            {
                _useCase.PresenterCallback?.OnError(error);
            }

            public override void OnCanceled(ZResponse<GetExcludedPackagesResponse> response)
            {
                _useCase.PresenterCallback?.OnCanceled(response);
            }

            public override void OnProgress(ZResponse<GetExcludedPackagesResponse> response)
            {
                _useCase.PresenterCallback?.OnProgress(response);
            }
        }
    }
}