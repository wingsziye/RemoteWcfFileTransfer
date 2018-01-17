using System.Windows;
using System.Windows.Controls;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl,IView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public Window BaseWindow { get; set; }

        public IViewModel ViewModel
        {
            get => (IViewModel)this.DataContext;
            set => this.DataContext = value;
        }
    }
}
