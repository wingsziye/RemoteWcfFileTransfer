using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Client2Server.Interfaces
{
    public class FileUpdateServiceHost : ServiceHost
    {
        private static FileUpdateServiceHost _updateHost;

        private readonly ServiceHost _host;
        private FileUpdateServiceHost(ServiceHost host)
        {
            _host = host;
        }

        /// <summary>
        /// 使用默认的FileUpdateService初始化ServiceHost
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ServiceHost InitializeHost( params string[] address)
        {
            return InitializeHost<FileUpdateService>(address);
        }

        /// <summary>
        /// 使用自定义继承IFileUpdateService的类初始化ServiceHost
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ServiceHost InitializeHost<T>(params string[] address) where T : class, IFileUpdateService
        {
            var host = new ServiceHost(typeof(T));
            foreach (var s in address)
            {
                host.AddServiceEndpoint(typeof(IFileUpdateService), new NetTcpBinding(SecurityMode.None), s);
            }
            _updateHost = new FileUpdateServiceHost(host);
            return host;
        }

        public static bool OpenHost()
        {
            try
            {
                _updateHost._host.Open(TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public static bool CloseHost()
        {
            try
            {
                _updateHost._host.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
