using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicEvents;
using Remote.Infrastructure.PublicModels;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public class ProgressReceiveViewModel : BindableBase, IProgressReceiveViewModel
    {
        public IView View { get; set; }

        public ClientToken SelfToken { get; set; }

        public ProgressReceiveViewModel()
        {
            var pubsub = PubSubEvents.Singleton;
            pubsub.GetEvent<FileReceiveProgressChangedEvent>().Subscribe(ProgressUpdate);
            pubsub.GetEvent<FileReceiveProgressCompleteEvent>().Subscribe(ProgressComplete);
        }

        private void ProgressUpdate(FileReceiveProgressChangedEvent update)
        {
            try
            {
                var result = _progressMessageCollection.ToList().Find((i) => i.Title == update.ProgressMsg.Title && Math.Abs(i.MaxValue - update.ProgressMsg.MaxValue) < 0.1);
                if (result == null)
                {
                    _progressMessageCollection.Add(update.ProgressMsg);
                }
            }
            catch
            {

            }
        }

        private void ProgressComplete(FileReceiveProgressCompleteEvent update)
        {
            var result = _progressMessageCollection.ToList().Find((i) => i.Title == update.FileName);
            if (result != null)
            {
                result.ProgressValue = result.MaxValue;
                result.StateMsg = "传输完成，请放心食用！";
            }
        }
        
        private ObservableCollection<ProgressMessage> _progressMessageCollection =
            new ObservableCollection<ProgressMessage>();

        public ObservableCollection<ProgressMessage> ProgressCollection
        {
            get => _progressMessageCollection;
            set => SetProperty(ref _progressMessageCollection, value);
        }
    }
}
