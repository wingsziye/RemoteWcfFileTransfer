using System.ComponentModel;

namespace Wpf.Infrastructure.MVVM
{
    public interface IViewModel : INotifyPropertyChanged
    {
        IView View { get; set; }
    }
}
