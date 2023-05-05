using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Server
{
    public class Program
    {
        public static void Main()
        {
            Uri baseAddress = new Uri("http://10.128.35.12:9090/");
            using (ServiceHost serviceHost = new ServiceHost(typeof(FileTransfer), baseAddress))
            {
                try 
                {
                    ServiceDebugBehavior debug = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();

                    // if not found - add behavior with setting turned on 
                    if (debug == null)
                    {
                        serviceHost.Description.Behaviors.Add(
                            new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                    }
                    else
                    {
                        // make sure setting is turned ON
                        if (!debug.IncludeExceptionDetailInFaults)
                        {
                            debug.IncludeExceptionDetailInFaults = true;
                        }
                    }

                    serviceHost.Open();

                    Console.WriteLine("The service is ready.");
                    Console.WriteLine("Press <ENTER> to terminate service.");
                    Console.ReadLine();

                    serviceHost.Close();
                }
                catch (TimeoutException timeProblem)
                {
                    Console.WriteLine(timeProblem.Message);
                    Console.ReadLine();
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine(commProblem.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}