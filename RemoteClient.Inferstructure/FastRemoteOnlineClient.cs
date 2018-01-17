using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemoteClient.Inferstructure
{
    public class FastRemoteOnlineClient : RemoteOnlineClientProxy
    {
        public enum FastClientTarget
        {
            LocalTest,
            Remote,
            Cloud
        }

        public FastRemoteOnlineClient() :base(new InstanceContext(new RemoteServicePush()), "NetTcpBinding_IRemoteOnlineService")
        {

        }

        public FastRemoteOnlineClient(string address) : base(new InstanceContext(new RemoteServicePush()),
            new NetTcpBinding(SecurityMode.None), address)
        {

        }

        public static FastRemoteOnlineClient CreateFastClient(string appSettingAddressKey)
        {
            return new FastRemoteOnlineClient(ConfigurationManager.AppSettings.Get(appSettingAddressKey));
        }

        public static FastRemoteOnlineClient CreateFastClient(FastClientTarget target)
        {
            switch (target)
            {
                case FastClientTarget.LocalTest:
                    return new FastRemoteOnlineClient("net.tcp://localhost:5566/");
                case FastClientTarget.Remote:
                    return CreateFastClient("RemoteAddress");
                case FastClientTarget.Cloud:
                    return CreateFastClient("CloudAddress");
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }
    }
}
