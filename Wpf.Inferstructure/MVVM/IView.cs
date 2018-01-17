using System.Windows;

namespace Wpf.Infrastructure.MVVM
{
    public interface IView
    {
        Window BaseWindow { get; }

        IViewModel ViewModel { get; set; }

        object DataContext { get; set; }
    }
}
