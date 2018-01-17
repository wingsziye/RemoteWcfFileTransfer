using Microsoft.Practices.Unity;
using Prism.Regions;
using RemoteClient.Beta.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.Views
{
    /// <summary>
    /// UserOnlineView.xaml 的交互逻辑
    /// </summary>
    public partial class UserOnlineView : UserControl,IView
    {
        private IRegionManager _manager;
        private IUnityContainer _container;
        public UserOnlineView(IUserOnlineViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
        }

        public Window BaseWindow { get; set; }

        public IViewModel ViewModel
        {
            get => (IViewModel)this.DataContext;
            set => this.DataContext = value;
        }
    }
}
