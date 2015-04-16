using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public interface IViewModelBase
    {
        string PageContainer { get; set; }
        string PageTitle { get; set; }
    }

    public class ViewModelBase : IViewModelBase
    {
        public string PageContainer { get; set; }
        public string PageTitle { get; set; }
    }
}

