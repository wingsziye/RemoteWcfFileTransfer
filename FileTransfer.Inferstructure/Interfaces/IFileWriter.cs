using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IFileWriter
    {
        /// <summary>
        /// 写文件 Write byte data to file
        /// </summary>
        /// <param name="transferData">writing result</param>
        /// <returns></returns>
        FileTransferResponsed WriteFile(FileTransferRequest transferData);

        /// <summary>
        /// 检查文件块信息
        /// </summary>
        /// <param name="blockMessage">反馈</param>
        /// <returns></returns>
        BlockTransferResponsed CheckBlockMessage(BlockTransferRequest blockMessage);
    }
}
