
namespace JCarrollOnlineV2.ViewModels
{
    public interface IViewModelBase
    {
        string PageContainer { get; set; }
        string PageTitle { get; set; }
        string Message { get; set; }
    }

    public class ViewModelBase : IViewModelBase
    {
        public string PageContainer { get; set; }
        public string PageTitle { get; set; }
        public string Message { get; set; }
    }
}

