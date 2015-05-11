using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace JCarrollOnlineV2.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class JCarrollOnlineV2Service : IJCarrollOnlineV2Service
    {
        public bool Login(string userName, string password)
        {
           System.Diagnostics.Debug.WriteLine(string.Format("You entered: {0},{1}", userName, password));
           return true;
        }
    }
}
