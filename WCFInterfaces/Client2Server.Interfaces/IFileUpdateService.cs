using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Remote.Infrastructure.DataContracts;

namespace Client2Server.Interfaces
{
    [ServiceContract]
    public interface IFileUpdateService
    {
        [OperationContract]
        FileTransferResponsed UpdateFile(FileTransferRequest transferData);

        [OperationContract]
        BlockTransferResponsed UpdateFileBlockCheckMessage(BlockTransferRequest blockMessage);
    }
}
