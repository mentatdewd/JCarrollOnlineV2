using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCarrollOnlineV2.WCFClient.JCarrollOnlineV2Service;

namespace JCarrollOnlineV2.WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Step 1: Create an instance of the WCF proxy.
            JCarrollOnlineV2ServiceClient client = new JCarrollOnlineV2ServiceClient();

            string userName = "John";
            string password = "password";

            bool result = client.Login(userName, password);
        }
    }
}
