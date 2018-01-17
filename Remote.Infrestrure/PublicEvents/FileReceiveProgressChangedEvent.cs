using Remote.Infrastructure.PublicModels;

namespace Remote.Infrastructure.PublicEvents
{
    public class FileReceiveProgressChangedEvent
    {
        private ProgressMessage _progressMsg;

        public ProgressMessage ProgressMsg
        {
            get { return _progressMsg; }
            set { _progressMsg = value; }
        }
    }
}
