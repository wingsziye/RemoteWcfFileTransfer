using System.ComponentModel;
using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class UpdateProgressToken : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void UpdateProgress(int progress)
        {
            this.TransProgress = progress;
        }

        private string _fileMd5;
        private int _transProgress;
        private int _maxValue;

        [DataMember]
        public string FileMd5
        {
            get { return _fileMd5; }
            set
            {
                _fileMd5 = value;
                OnPropertyChanged(nameof(FileMd5));
            }
        }

        [DataMember]
        public int TransProgress
        {
            get { return _transProgress; }
            set { _transProgress = value; OnPropertyChanged(nameof(TransProgress)); }
        }

        [DataMember]
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; OnPropertyChanged(nameof(MaxValue)); }
        }
    }
}
