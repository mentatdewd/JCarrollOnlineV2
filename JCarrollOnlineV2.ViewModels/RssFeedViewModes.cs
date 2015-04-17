using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class RssFeedViewModelBase : ViewModelBase
    {
    }

    public class RssFeedItemViewModel : RssFeedViewModelBase
    {
        [DataType(DataType.Url)]
        public string Link { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public string Title { get; set; }
    }

    public class RssFeedViewModel : RssFeedViewModelBase
    {
        public List<RssFeedItemViewModel> RssFeedItems { get; set; }
    }
}
