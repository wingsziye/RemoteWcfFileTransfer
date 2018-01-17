using System.Collections.Generic;
using Remote.Infrastructure.PublicModels;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IHandleFileSendProgress
    {
        Dictionary<string,ProgressMessage> ProgressMessageDic { get; set; }
        void OnSendStart(string progressName, ProgressMessage progress);
        void OnSending(string progressName);
        void OnSendEnd(string progressName, bool state);
    }
}
