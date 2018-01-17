using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Client2Server.Interfaces;

namespace TestHostClient2ServerInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(FileUpdateService)))
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
