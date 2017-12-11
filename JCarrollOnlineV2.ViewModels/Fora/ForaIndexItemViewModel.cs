using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class ForaIndexItemViewModel : ForaViewModelBase
    {
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Thread Count")]
        public int ThreadCount { get; set; }

        [Display(Name = "Last Thread")]
        public LastThreadViewModel LastThread { get; set; }
    }
}
