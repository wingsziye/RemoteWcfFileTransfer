
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IFileSendAdapter
    {
        /// <summary>
        /// 局域网传输
        /// </summary>
        /// <param name="transferData"></param>
        /// <returns></returns>
        FileTransferResponsed UpdateFileData(FileTransferRequest transferData);

        BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage);

    }
}
