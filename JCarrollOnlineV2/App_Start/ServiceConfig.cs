using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using JCarrollOnlineV2.WCFService;

namespace JCarrollOnlineV2.App_Start
{
    public class ServiceConfig
    {
        public static void StartService()
        {
            // Step 1 Create a URI to serve as the base address.
            Uri baseAddress = new Uri("http://localhost:44306/Service/");

            // Step 2 Create a ServiceHost instance
            ServiceHost selfHost = new ServiceHost(typeof(JCarrollOnlineV2Service), baseAddress);

            try
            {
                // Step 3 Add a service endpoint.
                selfHost.AddServiceEndpoint(typeof(JCarrollOnlineV2Service), new WSHttpBinding(), "CalculatorService");

                // Step 4 Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);
                selfHost.AddServiceEndpoint(
                            typeof(IMetadataExchange),
                            MetadataExchangeBindings.CreateMexHttpBinding(),
                            "http://localhost:44306/Service/mex");

                // Step 5 Start the service.
                selfHost.Open();
                //Console.WriteLine("The service is ready.");
                //Console.WriteLine("Press <ENTER> to terminate service.");
                //Console.WriteLine();
                //Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.
                //selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}