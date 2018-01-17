using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using RemoteClient.Beta.Settings;
using RemoteClient.Beta.ViewModels;
using RemoteClient.Beta.Views;

namespace RemoteClient.Beta.Modules
{
    [Module(OnDemand = true,ModuleName = nameof(ProgressModule))]
    public class ProgressModule : IModule
    {
        private IRegionManager _manager;
        private IUnityContainer _container;

        public ProgressModule(IRegionManager manager, IUnityContainer container)
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
            _container.RegisterType<ProgressReceiveView>();
            _container.RegisterType<ProgressSendingView>(new HierarchicalLifetimeManager());

            _container.RegisterType<ProgressSendingViewModel>(new HierarchicalLifetimeManager());
            _container.RegisterType<IProgressReceiveViewModel, ProgressReceiveViewModel>();

            //_manager.RegisterViewWithRegion(CoreRegionNames.ProgressSendingRegion, () => _container.Resolve<ProgressSendingView>());
        }
        private void AddViews()
        {
            var sendView = _container.Resolve<ProgressSendingView>();
            _manager.AddToRegion(CoreRegionNames.ProgressSendingRegion, sendView);
        }
    }
}
