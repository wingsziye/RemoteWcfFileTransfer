using Prism.Events;
using Prism.Mvvm;
using RemoteClient.Beta.Events;
using System.Collections.ObjectModel;
using Remote.Infrastructure.PublicModels;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public class ProgressSendingViewModel : BindableBase, IViewModel
    {
        private ObservableCollection<ProgressMessage> _progressCollection;
        private IEventAggregator _aggregator;
        public IView View { get; set; }

        public ObservableCollection<ProgressMessage> ProgressCollection
        {
            get { return _progressCollection; }
            set { SetProperty(ref _progressCollection, value); }
        }

        public ProgressSendingViewModel(IEventAggregator aggregator)
        {
            ProgressCollection = new ObservableCollection<ProgressMessage>();
            _aggregator = aggregator;
            _aggregator.GetEvent<FileSendStartEvent>().Subscribe(FileSendStart, ThreadOption.UIThread);
            _aggregator.GetEvent<FileSendProgressChangedEvent>().Subscribe(FileSendProgressChanged, ThreadOption.UIThread);
            _aggregator.GetEvent<FileSendOverEvent>().Subscribe(FileSendOver,ThreadOption.UIThread);
        }

        private void FileSendStart(ProgressMessage obj)
        {
            this.ProgressCollection.Add(obj);
        }

        private void FileSendProgressChanged(string obj)
        {
            
        }

        private void FileSendOver(string obj)
        {
            
        }
    }
}
