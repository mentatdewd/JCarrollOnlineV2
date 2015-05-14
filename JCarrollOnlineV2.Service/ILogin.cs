using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Service
{
    [ServiceContract(Namespace = "http://JCarrollOnlineV2.ServiceModel.Service")]
    public interface ILogin
    {
        [OperationContract]
        bool Login(string userName, string password);
    }
}
