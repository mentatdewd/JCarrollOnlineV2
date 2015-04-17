using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class MicropostViewModelBase : ViewModelBase
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }

    public class MicropostCreateViewModel : MicropostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } // :null => false
    }

    public class MicropostFeedViewModel : MicropostViewModelBase
    {
        public List<MicropostFeedItemViewModel> MicropostFeedItems { get; set; }
    }

    public class MicropostFeedItemViewModel : MicropostViewModelBase
    {
        public string Content { get; set; }
        public string UserName { get; set; }
    }
}
