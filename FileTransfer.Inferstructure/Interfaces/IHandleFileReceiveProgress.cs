using System.Collections.Generic;
using Remote.Infrastructure.PublicModels;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IHandleFileReceiveProgress
    {
        Dictionary<string, ProgressMessage> ProgressMessageDic { get; set; }
        void OnReceiveStart(string progressKey,ProgressMessage progress);
        void OnRecieving(string progressKey);
        void OnReceiveEnd(string progressKey, bool state);
    }
}
