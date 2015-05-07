using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class UserWelcomeViewModel : EmailViewModelBase
    {
        public string CallbackUrl { get; set; }
        //public bool IsPremiumUser { get; set; }
    }
}
