using System.Web.Mvc;

namespace JCarrollOnlineV2.ViewModels
{
    public class ErrorViewModel : ViewModelBase
    {
        public HandleErrorInfo HandleErrorInfo { get; set; }

        public ErrorViewModel()
        {
            PageTitle = "Error";
            PageContainer = "container-fluid";
        }

        public ErrorViewModel(HandleErrorInfo handleErrorInfo) : this()
        {
            HandleErrorInfo = handleErrorInfo;
        }
    }
}