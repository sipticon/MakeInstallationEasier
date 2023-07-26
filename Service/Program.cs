using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Service
{
    public class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Main()
        {
            Uri baseAddress = new Uri("http://localhost:9090/");
            using (ServiceHost serviceHost = new ServiceHost(typeof(FileTransfer), baseAddress))
            {
                try
                {
                    ServiceDebugBehavior debug = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                    if (debug == null)
                    {
                        serviceHost.Description.Behaviors.Add(
                            new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                    }
                    else
                    {
                        if (!debug.IncludeExceptionDetailInFaults)
                        {
                            debug.IncludeExceptionDetailInFaults = true;
                        }
                    }

                    serviceHost.Open();

                    log.Info("Service started successfully.");

                    Console.WriteLine("The service is ready.");
                    Console.WriteLine("Press <ENTER> to terminate service.");
                    Console.ReadLine();

                    serviceHost.Close();
                    log.Info("Service stopped successfully.");
                }
                catch (Exception ex)
                {
                    log.Error("Exception while opening service.",ex);
                    Console.ReadLine();
                }
            }
        }
    }
}