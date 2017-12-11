using JCarrollOnlineV2.ViewModels.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class AnonHomePageViewModelBase : ViewModelBase
    {

    }

    public class AnonHomePageViewModel : AnonHomePageViewModelBase
    {
        public BlogFeedViewModel BlogFeed { get; set; }
    }
}
