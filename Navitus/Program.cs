using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Threading.Tasks;
namespace Navitus
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static async Task Main(string[] args)
        {

            log.Debug("Starting REST API server...");

            // setting server
            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            var uri = new Uri("http://localhost:5000");

            // Starting nancy host server
            using (var host = new NancyHost(uri, new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                log.Info("REST API server is running on: " + uri);
                log.Info("Press enter to stop.");
                Console.ReadLine();
            }


        }
    }
}
