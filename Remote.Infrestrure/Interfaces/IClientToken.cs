using System.Collections.Generic;
using System.Net;
using Remote.Infrastructure.DataContracts;

namespace Remote.Infrastructure.Interfaces
{
    public interface IClientToken 
    {
        OnlineStateEnum OnlineState { get; set; }
        string IPv4StringAddress { get; set; }
        IPAddress Address { get; set; }
        int ServicePort { get; set; }
        string NickName { get; set; }
        bool IsDisposed { get; set; }
        List<string> AddedGroupList { get; set; }
        IClientToken PropertyCopy();
        bool AddNewGroup(string groupName, IErrMessageHandler errMsg);
        bool QuitGroup(string groupName, IErrMessageHandler errMsg);
        bool Equals(IClientToken other);
    }
}
