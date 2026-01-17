using System.Web.Mvc;

namespace JCarrollOnlineV2.ViewModels.Error
{
    public class ErrorViewModel : ViewModelBase
    {
        public ErrorInfo HandleErrorInfo { get; set; }

        public ErrorViewModel()
        {
            PageTitle = "Error";
            PageContainer = "container-fluid";
        }

        public ErrorViewModel(ErrorInfo handleErrorInfo) : this()
        {
            HandleErrorInfo = handleErrorInfo;
        }
    }
}