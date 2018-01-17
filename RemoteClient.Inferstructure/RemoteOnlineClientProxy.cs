using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;
using RemoteClient.Inferstructure.WCF.RemoteOnlineService;

namespace RemoteClient.Inferstructure
{
    public class RemoteOnlineClientProxy : ClientBase<IRemoteOnlineService>, IRemoteOnlineService
    {
        public RemoteOnlineClientProxy(string endpointName) : base(endpointName)
        {
        }

        public RemoteOnlineClientProxy(Binding binding, string address) : base(binding, new EndpointAddress(address))
        {
        }

        public RemoteOnlineClientProxy(InstanceContext callback, string endpointConfigurationName) : base(callback,endpointConfigurationName)
        {
        }

        public RemoteOnlineClientProxy(InstanceContext callback, Binding binding, string remoteAddress) : base(callback,binding,new EndpointAddress(remoteAddress))
        {
        }

        public List<ClientToken> GetOnlineUsers(ClientToken whoIam)
        {
            return Channel.GetOnlineUsers(whoIam);
        }

        public Task<List<ClientToken>> GetOnlineUsersAsync(ClientToken whoIam)
        {
            return Channel.GetOnlineUsersAsync(whoIam);
        }

        public void UpdateWhoIam(ClientToken whoIam)
        {
            Channel.UpdateWhoIam(whoIam);
        }

        public Task UpdateWhoIamAsync(ClientToken whoIam)
        {
            return Channel.UpdateWhoIamAsync(whoIam);
        }

        public void UpdateTokenOnlineState(ClientToken token)
        {
            Channel.UpdateTokenOnlineState(token);
        }

        public Task UpdateTokenOnlineStateAsync(ClientToken token)
        {
            return Channel.UpdateTokenOnlineStateAsync(token);
        }

        public List<string> GetExistGroupName(ClientToken whoIam)
        {
            return Channel.GetExistGroupName(whoIam);
        }

        public Task<List<string>> GetExistGroupNameAsync(ClientToken whoIam)
        {
            return Channel.GetExistGroupNameAsync(whoIam);
        }

        public bool TryConnectToAnotherClient(ClientToken target)
        {
            return Channel.TryConnectToAnotherClient(target);
        }

        public Task<bool> TryConnectToAnotherClientAsync(ClientToken target)
        {
            return Channel.TryConnectToAnotherClientAsync(target);
        }

        public bool BeginTranslateFile(ClientToken target)
        {
            return Channel.BeginTranslateFile(target);
        }

        public Task<bool> BeginTranslateFileAsync(ClientToken target)
        {
            return Channel.BeginTranslateFileAsync(target);
        }

        public FileTransferResponsed UpdateFileData(int prot, FileTransferRequest request)
        {
            return Channel.UpdateFileData(prot, request);
        }

        public Task<FileTransferResponsed> UpdateFileDataAsync(int prot, FileTransferRequest request)
        {
            return Channel.UpdateFileDataAsync(prot, request);
        }

        public BlockTransferResponsed UpdateFileBlockData(int port, BlockTransferRequest request)
        {
            return Channel.UpdateFileBlockData(port, request);
        }

        public Task<BlockTransferResponsed> UpdateFileBlockDataAsync(int port, BlockTransferRequest request)
        {
            return Channel.UpdateFileBlockDataAsync(port, request);
        }

        public bool EndTranslateFile(ClientToken target)
        {
            return Channel.EndTranslateFile(target);
        }

        public Task<bool> EndTranslateFileAsync(ClientToken target)
        {
            return Channel.EndTranslateFileAsync(target);
        }
    }
}
