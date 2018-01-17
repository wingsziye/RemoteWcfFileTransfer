using System.ComponentModel;

namespace Remote.Infrastructure.PublicModels
{
    public class ProgressMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name));
        }

        private double _progressValue;
        private double _maxValue;
        private string _title;
        private string _stateMsg;

        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; OnPropertyChanged(nameof(MaxValue)); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string StateMsg
        {
            get { return _stateMsg; }
            set { _stateMsg = value; OnPropertyChanged(nameof(StateMsg)); }
        }
    }
}
