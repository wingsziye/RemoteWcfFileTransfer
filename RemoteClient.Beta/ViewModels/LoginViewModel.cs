using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using RemoteClient.Beta.Settings;
using RemoteClient.Beta.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using FileTransfer.WcfClient;
using Remote.Infrastructure.DataContracts;
using RemoteClient.Inferstructure;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public class LoginViewModel : BindableBase, IViewModel,INavigationAware
    {
        private IUserOnlineViewModel _onlineViewModel;
        private IEventAggregator _aggregator;
        private IRegionManager _regionManager;
        private FastRemoteOnlineClient.FastClientTarget _selectedFastClientType;

        public IView View { get; set; }

        public IEnumerable<OnlineStateEnum> OnlineStateEnums => Enum.GetValues(typeof(OnlineStateEnum)).Cast<OnlineStateEnum>();

        public IEnumerable<FastRemoteOnlineClient.FastClientTarget> FastClientTypes => Enum.GetValues(typeof(FastRemoteOnlineClient.FastClientTarget)).Cast<FastRemoteOnlineClient.FastClientTarget>();

        private ClientToken _loginToken;

        #region Properties

        public ClientToken LoginToken
        {
            get { return _loginToken; }
            set
            {
                SetProperty(ref _loginToken, value);
                _onlineViewModel.SelfToken = _loginToken;
            }
        }
        public FastRemoteOnlineClient.FastClientTarget SelectedFastClientType
        {
            get { return _selectedFastClientType; }
            set { SetProperty(ref _selectedFastClientType, value); }
        }
        #endregion


        #region Commands

        public DelegateCommand ConnectServerCommand { get; set; }

        #endregion

        public LoginViewModel(IEventAggregator aggregator,IRegionManager regionManager,IUserOnlineViewModel userOnline)
        {
            _aggregator = aggregator;
            _regionManager = regionManager;
            
            this._onlineViewModel = userOnline;
            this._onlineViewModel.OnlineClients = new ObservableCollection<ClientToken>();

            LoginToken = ClientTokenCreator.Create();
            int port = int.Parse(ConfigurationManager.AppSettings.Get("UploadPort"));
            LoginToken.ServicePort = port;

            ConnectServerCommand = new DelegateCommand(ConnectServer);
        }

        #region CommandMethods

        private void ConnectServer()
        {
            _onlineViewModel.ClientProxy = FastRemoteOnlineClient.CreateFastClient(SelectedFastClientType);

            var parameters = new NavigationParameters();
            _regionManager.RequestNavigate
            (CoreRegionNames.MainRegion, typeof(UserOnlineView).FullName,
                parameters);
        }

        #endregion

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _onlineViewModel.ClientProxy.Close();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
