﻿using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class EmailViewModelBase
    {
        public ApplicationUserViewModel TargetUser { get; set; }
        public string Content { get; set; }
    }
}