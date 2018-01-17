using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.Interfaces;
using Remote.Infrastructure.PublicEvents;
using Remote.Infrastructure.Settings;

namespace Server2Client.Interfaces
{
    public class RemoteOnlineService : IRemoteOnlineService,IDisposable
    {
        public RemoteOnlineService()
        {
            _pubsub = PubSubEvents.Singleton;
            _clientGroupList = ClientGroupCollection.Instance;
            _clientGroupList[MagicStrings.DefaultGroup].OnlineClients = DefaultClientList;
            _remoteUpdateSenderAdapterDic = new Dictionary<int, FileSendProxy>();
            _pushService = OperationContext.Current.GetCallbackChannel<IPushServiceCallback>();
            _tokenList = new ClientTokenList();
            ServerPushList.Add(_pushService);
            var scEvent = new ServiceCreatedEvent<RemoteOnlineService>(this);
            _pubsub.Publish(scEvent);
        }

        #region 字段

        private readonly IClientGroupCollection _clientGroupList;

        private readonly IPushServiceCallback _pushService;

        private readonly PubSubEvents _pubsub;

        private readonly ClientTokenList _tokenList;

        private readonly Dictionary<int, FileSendProxy> _remoteUpdateSenderAdapterDic;


        #endregion

        #region 静态字段

        private static readonly Dictionary<ClientToken, IPushServiceCallback> PushServiceDic = new Dictionary<ClientToken, IPushServiceCallback>();

        public static readonly ClientTokenList DefaultClientList = new ClientTokenList();

        public static readonly List<IPushServiceCallback> ServerPushList = new List<IPushServiceCallback>();
        
        #endregion

        

        private void ClientTokenFirstUpdate(ClientTokenFirstUpdateEvent se)
        {
            var token = se.User as ClientToken;
            var servicePushList = from p in ServerPushList
                where p != _pushService
                select p;
            foreach (var push in servicePushList)
            {
                push.ServerPushUserOnlineStateChanged(token);
            }
        }

        /// <summary>
        /// 客户端的UserClient OnlineState处理事件，将该事件转发给其他客户端
        /// </summary>
        /// <param name="se"></param>
        private void UserOnlineStateChanged(ClientTokenOnlineStateChangedEvent se)
        {
            var token = se.User as ClientToken;
            //排除与该服务对象连接的客户端，防止客户端已断开
            var servicePushList = from p in ServerPushList
                where p != _pushService
                select p;
            foreach (var push in servicePushList)
            {
                push.ServerPushUserOnlineStateChanged(token);
            }
        }

        public List<ClientToken> GetOnlineUsers(ClientToken whoIam)
        {
            var copy = new List<ClientToken>();
            //var tokenList = from token in DefaultClientList
            //    where !token.Address.Equals(whoIam.Address) && token.ServicePort != whoIam.ServicePort
            //    select token;
            copy.AddRange(DefaultClientList);
            return copy;
        }

        public void UpdateWhoIam(ClientToken whoIam)
        {
            var client = DefaultClientList.Find(whoIam);//从默认组中找到client
            if (client == null)//若未找到则添加入组
            {
                var addedClient = DefaultClientList.AddNew(whoIam);
                this._tokenList.Add(addedClient);
                PushServiceDic.Add(addedClient, this._pushService);
                var even = new ClientTokenFirstUpdateEvent() {User = addedClient};
                ClientTokenFirstUpdate(even);
                _pubsub.Publish(even);
            }
        }

        public void UpdateTokenOnlineState(ClientToken token)
        {
            var client = DefaultClientList.Find(token);
            if (client != null)//若找到，则拷贝远端属性后发出事件通知
            {

                ClientToken.PropertyCopy(token, client);
                var even = new ClientTokenOnlineStateChangedEvent() {User = client};
                UserOnlineStateChanged(even);
                _pubsub.Publish(even);
            }
        }

        

        public List<string> GetExistGroupName(ClientToken whoIam)
        {
            List<string> groupNameList = new List<string>();
            for (int i = 0; i < _clientGroupList.Count; i++)
            {
                groupNameList.Add(_clientGroupList[i].GroupName);
            }
            return groupNameList;
        }

        private void ServiceDispose()
        {
            var servicePushList = from p in ServerPushList
                where p != _pushService
                select p;
            Console.WriteLine();
            foreach (var push in servicePushList)
            {
                foreach (var token in _tokenList)
                {
                    push.ServerPushUserOnlineStateChanged(token);
                }
            }
        }

        public bool TryConnectToAnotherClient(ClientToken target)
        {
            bool result = false;
            try
            {
                //TODO 找到Callback,存在或连接正常，则为True
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="target"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool BeginTranslateFile(ClientToken target)
        {
            //根据target找到相应的PushCallback对象
            var client = DefaultClientList.Find(target);
            if (client==null)
            {
                return false;
            }

            //通知客户端打开文件上传服务
            var pushService = PushServiceDic[client];
            pushService.ServerPushOpenUpdateService();
            _remoteUpdateSenderAdapterDic.Add(target.ServicePort, new FileSendProxy(pushService));

            Thread.Sleep(300);//等待远程服务开启
            try
            {
                //var proxy = FileUpdateClientProxy.CreateTcpProxy(target.GenServiceAddress());
                //proxy.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public bool EndTranslateFile(ClientToken target)
        {
            //根据target找到相应的PushCallback对象
            var client = DefaultClientList.Find(target);
            if (client == null)
            {
                return false;
            }
            //通知客户端关闭文件上传服务
            var callback = PushServiceDic[client];
            callback.ServerPushCloseUpdateService();
            _remoteUpdateSenderAdapterDic.Remove(target.ServicePort);//移除服务引用
            
            return true;
        }

        
        public FileTransferResponsed UpdateFileData(int port, FileTransferRequest request)
        {
            var sendAdapter = _remoteUpdateSenderAdapterDic[port];
            return sendAdapter.UpdateFileData(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BlockTransferResponsed UpdateFileBlockData(int port,BlockTransferRequest request)
        {
            var sendAdapter = _remoteUpdateSenderAdapterDic[port];
            return sendAdapter.UpdateFileBlockMessage(request);
        }

        public void Dispose()
        {
#if DEBUG
            Console.WriteLine("Service Disposed.A Client must offline");
#endif
            foreach (var token in _tokenList)
            {
                token.OnlineState = OnlineStateEnum.Offline;//将该连接的ClientToken全部置为离线状态。并不知道这么做有什么用
                PushServiceDic.Remove(token);
            }

            _remoteUpdateSenderAdapterDic.Clear();

            ServiceDispose();

            var sdEvent = new ServiceDisposeEvent<RemoteOnlineService>();
            _pubsub.Publish(sdEvent);
            ServerPushList.Remove(this._pushService);//处理完事件后再移除回调Instance
            
            foreach (var token in _tokenList)
            {
                DefaultClientList.Remove(token);
            }
        }
    }
}
