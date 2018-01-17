using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Remote.Infrastructure.DataContracts;

namespace Server2Client.Interfaces
{
    public interface IPushServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ServerPushUserOnlineStateChanged(ClientToken newUser);

        [OperationContract(IsOneWay = true)]
        void ServerPushServiceClosingCall();

        [OperationContract(IsOneWay = true)]
        void ServerPushOpenUpdateService();

        [OperationContract(IsOneWay = true)]
        void ServerPushCloseUpdateService();

        [OperationContract]
        FileTransferResponsed TransferFileData(FileTransferRequest transferData);

        [OperationContract]
        BlockTransferResponsed TransferFileBlockMessage(BlockTransferRequest blockMessage);
    }
}
