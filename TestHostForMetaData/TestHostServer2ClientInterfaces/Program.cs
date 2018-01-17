using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Server2Client.Interfaces;

namespace TestHostServer2ClientInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(RemoteOnlineService)))
            {
                host.Opened += (s, e) =>
                {
                    Console.WriteLine("Host is Open!");
                };
                host.Faulted += (s, e) =>
                {
                    Console.WriteLine("Host is Faulted!");
                };
                host.Closed += (s, e) =>
                {
                    Console.WriteLine("Host is Closed!");
                };
                host.Open();

                Console.WriteLine("Press Enter to exit!");
                Console.ReadLine();
            }
        }
    }
}
