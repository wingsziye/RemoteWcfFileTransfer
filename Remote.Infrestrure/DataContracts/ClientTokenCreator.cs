using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Infrastructure.DataContracts
{
    public class ClientTokenCreator
    {
        public static ClientToken Create(string tokenName = "DefaultUser", string nickName = "Nick")
        {
            ClientToken token = new ClientToken();
            token.Address = GetIpAddress();
            token.IPv4StringAddress = token.Address.MapToIPv4().ToString();
            token.OnlineState = OnlineStateEnum.Online;
            token.NickName = nickName;
            return token;
        }

        private static IPAddress GetIpAddress()
        {
            string host = Dns.GetHostName();
            var ipEntry = Dns.GetHostAddresses(host);
            foreach (var ipAddress in ipEntry)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }
            return null;
        }
    }
}
