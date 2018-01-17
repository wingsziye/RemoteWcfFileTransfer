using Remote.Infrastructure.PublicModels;
using RemoteClient.Beta.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.Views
{
    /// <summary>
    /// ProgressSendingView.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressSendingView : UserControl, IView
    {
        public ProgressSendingView(ProgressSendingViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
        }

        public Window BaseWindow { get; set; }

        public IViewModel ViewModel
        {
            get => (IViewModel) this.DataContext;
            set => this.DataContext = value;
        }

        private ObservableCollection<ProgressMessage> _progressMessageCollection =
            new ObservableCollection<ProgressMessage>();

        public ObservableCollection<ProgressMessage> ProgressMessageCollection
        {
            get => _progressMessageCollection;
            set => _progressMessageCollection = value;
        }
    }
}
