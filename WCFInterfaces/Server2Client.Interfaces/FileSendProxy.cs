using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Infrastructure.DataContracts;

namespace Server2Client.Interfaces
{
    class FileSendProxy
    {
        private IPushServiceCallback _pushService;
        public FileSendProxy(IPushServiceCallback callback)
        {
            _pushService = callback;
        }

        public FileTransferResponsed UpdateFileData(FileTransferRequest transferData)
        {
            return _pushService.TransferFileData(transferData);
        }

        public BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage)
        {
            return _pushService.TransferFileBlockMessage(blockMessage);
        }
    }
}
