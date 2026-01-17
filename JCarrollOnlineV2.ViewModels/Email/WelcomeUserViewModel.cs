using System;

namespace JCarrollOnlineV2.ViewModels.Email
{
    public class UserWelcomeViewModel : EmailViewModelBase
    {
        public Uri CallbackUrl { get; set; }
        //public bool IsPremiumUser { get; set; }
    }
}
