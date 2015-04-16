using JCarrollOnlineV2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public Micropost Micropost { get; set; }
        public virtual ICollection<Micropost> Microposts { get; set; }
    }
}
