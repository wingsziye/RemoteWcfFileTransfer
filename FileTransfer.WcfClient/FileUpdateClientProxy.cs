using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure.Interfaces;
using FileTransfer.WcfClient.WCF.FileUpdateService;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.WcfClient
{
    public class FileUpdateClientProxy : ClientBase<IFileUpdateService>,IFileUpdateService,IFileSendAdapter
    {
        public FileUpdateClientProxy() : base() { }

        public FileUpdateClientProxy(string endpointName) : base(endpointName) { }

        public FileUpdateClientProxy(Binding binding, string address) : base(binding, new EndpointAddress(address)) { }

        public FileUpdateClientProxy(InstanceContext callback, string address) : base(callback, address) { }

        public static FileUpdateClientProxy CreateTcpProxy(string endPoint)
        {
            return new FileUpdateClientProxy(new NetTcpBinding(SecurityMode.None), endPoint);
        }

        public FileTransferResponsed UpdateFile(FileTransferRequest transferData)
        {
            return Channel.UpdateFile(transferData);
        }

        public Task<FileTransferResponsed> UpdateFileAsync(FileTransferRequest transferData)
        {
            return Channel.UpdateFileAsync(transferData);
        }

        public BlockTransferResponsed UpdateFileBlockCheckMessage(BlockTransferRequest blockMessage)
        {
            return Channel.UpdateFileBlockCheckMessage(blockMessage);
        }

        public Task<BlockTransferResponsed> UpdateFileBlockCheckMessageAsync(BlockTransferRequest blockMessage)
        {
            return Channel.UpdateFileBlockCheckMessageAsync(blockMessage);
        }

        public FileTransferResponsed UpdateFileData(FileTransferRequest transferData)
        {
            return Channel.UpdateFile(transferData);
        }

        public BlockTransferResponsed UpdateFileBlockMessage(BlockTransferRequest blockMessage)
        {
            return Channel.UpdateFileBlockCheckMessage(blockMessage);
        }
    }
}
