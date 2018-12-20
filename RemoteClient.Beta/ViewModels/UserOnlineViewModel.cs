using Client2Server.Interfaces;
using FileTransfer.Infrastructure.Interfaces;
using FileTransfer.WcfClient;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicModels;
using Remote.Infrastructure.Tookies;
using RemoteClient.Beta.Events;
using RemoteClient.Beta.Modules;
using RemoteClient.Beta.Settings;
using RemoteClient.Beta.Views;
using RemoteClient.Inferstructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using FileTransfer.Infrastructure;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public class UserOnlineViewModel : BindableBase, IUserOnlineViewModel, INavigationAware, IHandleFileSendProgress, IHandleUpdateResponsed, IDisposable
    {
        public IView View { get; set; }
        private IEventAggregator _aggregator;
        private IRegionManager _regionManager;
        private IModuleManager _modules;
        private IUnityContainer _container;

        public RemoteOnlineClientProxy ClientProxy { get; set; }
        public ClientToken SelfToken { get; set; }
        public ObservableCollection<ClientToken> OnlineClients { get; set; }
        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand<object> UserSelectCommand { get; set; }
        public Dictionary<string, ProgressMessage> ProgressMessageDic { get; set; }

        public UserOnlineViewModel(IEventAggregator aggregator, IRegionManager regionManager, IModuleManager modules, IUnityContainer container)
        {
            _aggregator = aggregator;
            _regionManager = regionManager;
            _modules = modules;
            _container = container;

            LogoutCommand = new DelegateCommand(Logout, CanLogout);
            UserSelectCommand = new DelegateCommand<object>(UserSelect);

            ProgressMessageDic = new Dictionary<string, ProgressMessage>();

            SubscribeRemoteServicePushEvents();
        }

        /// <summary>
        /// 订阅RemoteServicePush的事件
        /// </summary>
        private void SubscribeRemoteServicePushEvents()
        {
            RemoteServicePush.UserOnlineStateChanged += RemoteServicePush_UserOnlineStateChanged;//远程在线用户状态改变事件，新用户上线也使用此事件
            RemoteServicePush.ServiceClosing += RemoteServicePush_ServiceClosing;//远程服务被关闭事件
            RemoteServicePush.ServerCallOpenUpdateService += RemoteServicePush_ServerCallOpenUpdateService;//打开文件上传服务事件
            RemoteServicePush.ServerCallCloseUpdateService += RemoteServicePush_ServerCallCloseUpdateService;//关闭文件上传服务事件
        }

        private void RemoteServicePush_ServerCallCloseUpdateService()
        {
            Console.WriteLine("ServerCallCloseUpdateService closing");
            Task.Run(() =>
            {
                FileUpdateServiceHost.CloseHost();
            });
        }

        private void RemoteServicePush_ServerCallOpenUpdateService()
        {
            Console.WriteLine("ServerCallOpenUpdateService Initlizing");
            var host = FileUpdateServiceHost.InitializeHost(SelfToken.GenServiceAddress());
            host.Opened += Host_Opened;
            host.Closed += Host_Closed;
            host.Faulted += Host_Faulted;

            FileUpdateServiceHost.OpenHost();
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            //TODO : 关闭文件接收进度Dialog，如果还在的话
            Console.WriteLine("FileUpdateService Closed");
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            //TODO : 找个方法提示错误
            Console.WriteLine("FileUpdateService Faulted");
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            //TODO : 打开文件接收进度Dialog
            Console.WriteLine("FileUpdateService Opened");
            var view = _container.Resolve<ProgressReceiveView>();
            var viewModel = _container.Resolve<IProgressReceiveViewModel>();
            view.ViewModel = viewModel;
            var window = new ProgressReceiveWindow();
            view.BaseWindow = window;
            window.AddContent(view);
            view.BaseWindow.Show();
        }

        private void RemoteServicePush_ServiceClosing()
        {
            foreach (var client in OnlineClients)
            {
                client.OnlineState = OnlineStateEnum.Offline;
            }
        }

        private void RemoteServicePush_UserOnlineStateChanged(ClientToken obj)
        {
            Console.WriteLine($"RemotePush {obj.NickName} {obj.OnlineState}");
            var client = ClientTokenList.Find(OnlineClients.ToList(), obj);
            if (client == null)
            {
                OnlineClients.Add(obj);
            }
            else
            {
                ClientToken.PropertyCopy(obj, client);
            }
        }

        private bool CanLogout()
        {
            return true;
        }

        private void Logout()
        {
            _regionManager.RequestNavigate(CoreRegionNames.MainRegion, typeof(LoginView).FullName);
        }

        private async void UserSelect(object obj)
        {
            var collection = obj as IList;
            if (collection == null)
            {
                return;
            }
            var fileArray = FileTookit.FileDialogSelect();
            var taskList = new List<Task>();
            foreach (var t in collection)
            {
                //TODO : 1 先Begin，让Service Push Call目标客户端开启文件传输服务
                //TODO : 2 再直接建立连接进行局域网测试，若测试失败，则使用服务端进行Try测试，若测试均失败，则return 退出传输,最好能报个错误
                ClientToken targetToken = t as ClientToken;
                var task = new Task(() =>
                {
                    if (targetToken.OnlineState == OnlineStateEnum.Online)
                    {
                        if (!BeginRemoteUpdateService(targetToken))
                        {
                            return;
                        }
                        Console.WriteLine("开始测试LAN连接!");
                        bool lanConnectResult = LanFileUpdateTest(targetToken);

                        if (!lanConnectResult)
                        {
                            Console.WriteLine("局域网连接测试失败了！！");
                        }

                        RunFileTransfer(lanConnectResult, fileArray, targetToken.ServicePort, ClientProxy, targetToken);

                        if (!EndRemoteUpdateService(targetToken))
                        {
                            throw new Exception("关闭远程服务出错");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"在想什么呢？没看到{targetToken.NickName}不在线吗？");
                    }
                });
                taskList.Add(task);
                task.Start();
            }
            await Task.WhenAll(taskList.ToArray());
        }

        private void RunFileTransfer(bool isLan, string[] filePathList, int targetPort, RemoteOnlineClientProxy proxy, ClientToken targetToken)
        {
            if (isLan)
            {
                LanFileListTransfer(targetToken, filePathList);
            }
            else
            {
#if DEBUG
                Console.WriteLine("远程服务转发模式");
#endif
                FileTransferList(filePathList, targetPort, proxy);
            }
        }

        private void FileTransferList(string[] filePathList, int targetPort, RemoteOnlineClientProxy proxy)
        {
            List<Task> taskList = new List<Task>();
            foreach (var path in filePathList)
            {
                Task t = new Task(() =>
                {
                    var reader = new LocalFileReader(new FileRemoteSendAdapter(targetPort, proxy), this, this);
                    reader.RunFileTransfer(path);
                });
                taskList.Add(t);
                t.Start();
            }
            Task.WaitAll(taskList.ToArray());//使用多线程上传文件
        }

        private void LanFileListTransfer(ClientToken targetToken, string[] filePathList)
        {
            IFileSender adapter = FileUpdateClientProxy.CreateTcpProxy(targetToken.GenServiceAddress());
            var easyTransfer = new LocalFileReader(adapter, this, this);

            List<Task> taskList = new List<Task>();
            foreach (var path in filePathList)
            {
                Task t = new Task(() =>
                {
                    easyTransfer.RunFileTransfer(path);//这个接口目前只支持局域网
                });
                taskList.Add(t);
                t.Start();
            }
            Task.WaitAll(taskList.ToArray());//使用多线程上传文件
        }

        private bool LanFileUpdateTest(ClientToken targetToken)
        {
            try
            {
                using (FileUpdateClientProxy proxy = new FileUpdateClientProxy(new NetTcpBinding(SecurityMode.None), targetToken.GenServiceAddress()))
                {
                    proxy.Open();
                }
#if DEBUG
                Console.WriteLine("LAN connection");
#endif
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        private bool BeginRemoteUpdateService(ClientToken targetToken)
        {
            var beginResponse = ClientProxy.BeginTranslateFile(targetToken);
            if (!beginResponse)
            {
                Console.WriteLine("开始传输时发生错误");
                return false;
            }
            return true;
        }

        private bool EndRemoteUpdateService(ClientToken targetToken)
        {
            var endResponse = ClientProxy.EndTranslateFile(targetToken);
            if (!endResponse)
            {
                Console.WriteLine("结束传输时发生错误");
                return false;
            }
            return true;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Console.WriteLine("From");
            SelfToken.OnlineState = OnlineStateEnum.Offline;
            ClientProxy.UpdateWhoIam(SelfToken);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Console.WriteLine("OnNavigatedTo");
            _modules.LoadModule(nameof(ProgressModule));
            Console.WriteLine(SelfToken.OnlineState);
            ClientProxy.Open();
            ClientProxy.UpdateWhoIam(SelfToken);
            OnlineClients.Clear();
            this.OnlineClients.AddRange(ClientProxy.GetOnlineUsers(SelfToken));
        }

        public void HandleResponsed(FileTransferResponsed responsed)
        {
            //throw new NotImplementedException();
        }

        public void OnSendStart(string progressName, ProgressMessage progress)
        {
            this._aggregator.GetEvent<FileSendStartEvent>().Publish(progress);
        }

        public void OnSending(string progressName)
        {
            this._aggregator.GetEvent<FileSendProgressChangedEvent>().Publish(progressName);
        }

        public void OnSendEnd(string progressName, bool state)
        {
            this._aggregator.GetEvent<FileSendOverEvent>().Publish(progressName);
        }

        public void Dispose()
        {
            ClientProxy.Close();
            ((IDisposable)ClientProxy)?.Dispose();
        }
    }
}