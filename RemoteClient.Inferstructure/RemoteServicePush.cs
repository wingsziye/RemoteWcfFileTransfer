using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicEvents;
using Remote.Infrastructure.PublicModels;
using RemoteClient.Inferstructure.WCF.RemoteOnlineService;

namespace RemoteClient.Inferstructure
{
    public class RemoteServicePush : IRemoteOnlineServiceCallback,IHandleFileReceiveProgress
    {
        public static event Action ServiceClosing;
        public static event Action<ClientToken> UserOnlineStateChanged;
        public static event Action ServerCallOpenUpdateService;
        public static event Action ServerCallCloseUpdateService;

        private LocalFileWriter _fileWriter;

        public RemoteServicePush()
        {
            _fileWriter = new LocalFileWriter(this);
            ProgressMessageDic=new Dictionary<string, ProgressMessage>();
        }

        public void ServerPushUserOnlineStateChanged(ClientToken token)
        {
            UserOnlineStateChanged?.Invoke(token);
            Console.WriteLine($"{token.NickName} {token.OnlineState}");
        }

        public void ServerPushServiceClosingCall()
        {
            ServiceClosing?.Invoke();
        }

        public void ServerPushOpenUpdateService()
        {
            ServerCallOpenUpdateService?.Invoke();

        }

        public void ServerPushCloseUpdateService()
        {
            ServerCallCloseUpdateService?.Invoke();
        }

        public FileTransferResponsed TransferFileData(FileTransferRequest transferData)
        {
            return _fileWriter.WriteFile(transferData);
        }

        public BlockTransferResponsed TransferFileBlockMessage(BlockTransferRequest blockMessage)
        {
            return _fileWriter.CheckBlockMessage(blockMessage);
        }

        public Dictionary<string, ProgressMessage> ProgressMessageDic { get; set; }

        public void OnReceiveStart(string progressKey, ProgressMessage progress)
        {
            ProgressMessageDic?.Add(progressKey, progress);
        }

        public void OnRecieving(string progressKey)
        {
            try
            {
                if (ProgressMessageDic?.ContainsKey(progressKey) == true)
                {
                    PubSubEvents.Singleton.Publish(new FileReceiveProgressChangedEvent() { ProgressMsg = ProgressMessageDic[progressKey] });
                }
            }
            catch
            {
                // ignored
            }
        }

        public void OnReceiveEnd(string progressKey, bool state)
        {
            try
            {
                if (ProgressMessageDic?.ContainsKey(progressKey) == true)
                {
                    var progress = ProgressMessageDic[progressKey];
                    PubSubEvents.Singleton.Publish(
                        new FileReceiveProgressCompleteEvent() { FileName = progress.Title, IsChecked = state });
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
