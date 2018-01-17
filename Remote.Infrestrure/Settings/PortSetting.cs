using System.Configuration;

namespace Remote.Infrastructure.Settings
{
    public class PortSetting
    {
        public static int ServerLocalPort = 5666;
        public static int ServerLocalMetaPort = 5667;
        public static int ServerRemotePort = 12886;
        public static int ServerRemoteMetaPort = 12887;

        public static int ClientLocalPort = 5668;
        public static int ClientLocalMetaPort = 5669;
        public static int ClientRemotePort = 12888;
        public static int ClientRemoteMetaPort = 12889;

        public static void UpdateServerPortSetting(int remoteClientPort)
        {
            ConfigurationManager.RefreshSection("system.serviceModel");
            ConfigurationManager.RefreshSection("system.serviceModel/behaviors");
            ConfigurationManager.RefreshSection("system.serviceModel/bindings");
            //ConfigurationManager.RefreshSection("system.serviceModel/client");
            ConfigurationManager.RefreshSection("system.serviceModel/services");
        }


        public static void UpdateClientPortSetting()
        {
            
        }
    }
}
