using FileTransfer.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using RemoteClient.Beta.Settings;
using RemoteClient.Beta.ViewModels;
using RemoteClient.Beta.Views;

namespace RemoteClient.Beta.Modules
{
    public class CoreModule : IModule
    {
        private IRegionManager _manager;
        private IUnityContainer _container;

        public CoreModule(IRegionManager manager, IUnityContainer container)
        {
            _manager = manager;
            _container = container;
        }

        public void Initialize()
        {
            RegistTypes();
            AddViews();
        }

        private void RegistTypes()
        {
            _container.RegisterTypeForNavigation<LoginView>(typeof(LoginView).FullName);
            //_container.RegisterType(typeof(object),typeof(UserOnlineView), "UserOnlineView");
            _container.RegisterTypeForNavigation<UserOnlineView>(typeof(UserOnlineView).FullName);
            
            _container.RegisterType<LoginViewModel>(new HierarchicalLifetimeManager());
            _container.RegisterType<IUserOnlineViewModel, UserOnlineViewModel>(new HierarchicalLifetimeManager());

            _container.RegisterType<IHandleFileSendProgress, UserOnlineViewModel>(new HierarchicalLifetimeManager());
            _container.RegisterType<IHandleUpdateResponsed, UserOnlineViewModel>(new HierarchicalLifetimeManager());
            
        }

        private void AddViews()
        {
            var loginView = _container.Resolve<LoginView>();
            var loginViewModel = _container.Resolve<LoginViewModel>();
            loginView.ViewModel = loginViewModel;
            _manager.AddToRegion(CoreRegionNames.MainRegion, loginView);
        }
    }
}
