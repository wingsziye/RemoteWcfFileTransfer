using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Test
{
    class LocalFileSender : IFileSender
    {
        private IFileWriter writer;

        public LocalFileSender(IFileWriter writer)
        {
            this.writer = writer;
        }

        public FileTransferResponsed UpdateFileData(FileTransferRequest transferData)
        {
            return writer.WriteFile(transferData);
        }

        public BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage)
        {
            return writer.CheckBlockMessage(blockMessage);
        }
    }
}
