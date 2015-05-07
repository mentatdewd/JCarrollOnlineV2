﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class MicropostNotificationEmailViewModel : EmailViewModelBase
    {
        public ApplicationUserViewModel MicropostAuthor { get; set; }
        public string MicropostContent { get; set; }
    }
}
