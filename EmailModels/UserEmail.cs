using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.EmailModels
{
    public class UserEmail
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsPremiumUser { get; set; }
    }
}
