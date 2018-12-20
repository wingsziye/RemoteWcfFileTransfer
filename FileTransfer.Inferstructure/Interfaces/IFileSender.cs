
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IFileSender
    {
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="transferData"></param>
        /// <returns></returns>
        FileTransferResponsed UpdateFileData(FileTransferRequest transferData);

        /// <summary>
        /// 发送文件块信息
        /// </summary>
        /// <param name="blockMessage"></param>
        /// <returns></returns>
        BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage);

    }
}
