namespace Remote.Infrastructure.PublicEvents
{
    public class FileReceiveProgressCompleteEvent
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public bool IsChecked { get; set; }
    }
}
