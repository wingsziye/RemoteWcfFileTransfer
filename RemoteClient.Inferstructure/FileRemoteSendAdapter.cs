using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;

namespace RemoteClient.Inferstructure
{
    public class FileRemoteSendAdapter : IFileSender
    {
        private RemoteOnlineClientProxy _proxy;
        private int _targetPort;
        public FileRemoteSendAdapter(int targetPort,RemoteOnlineClientProxy proxy)
        {
            _targetPort = targetPort;
            _proxy = proxy;
        }

        public FileTransferResponsed UpdateFileData(FileTransferRequest transferData)
        {
            return _proxy.UpdateFileData(_targetPort,transferData);
        }

        public BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage)
        {
            return _proxy.UpdateFileBlockData(_targetPort, blockMessage);
        }
    }
}
