using System.ComponentModel;

namespace Wpf.Infrastructure.MVVM
{
    public class ViewModelBase : IViewModel
    {
        public virtual IView View { get; set; }

        public ViewModelBase(IView view)
        {
            this.View = view;
            this.View.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
