using System.ComponentModel;
using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class FileTransferResponsed : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FileTransferResponsed()
        {
        }

        public FileTransferResponsed(FileTransferRequest token)
        {
            
        }

        private bool _isWait = false;
        private long _remoteStreamPosition;
        private bool _isSendingOver = false;
        private bool _fileMd5CheckResult = false;
        private int _requestId;//上传方生成的随机值
        private bool _isError;//上传过程是否出现错误
        private string _errMsg;//错误信息

        [DataMember]
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                _isWait = value;
                OnPropertyChanged(nameof(IsWait));
            }
        }

        [DataMember]
        public bool IsSendingOver
        {
            get { return _isSendingOver; }
            set { _isSendingOver = value; }
        }

        [DataMember]
        public long RemoteStreamPosition
        {
            get { return _remoteStreamPosition; }
            set
            {
                _remoteStreamPosition = value; OnPropertyChanged(nameof(RemoteStreamPosition));
            }
        }

        /// <summary>
        /// 当Md5校验失败和文件传输完成时，重传
        /// </summary>
        public bool IsNeedReSend
        {
            get => !FileMd5CheckResult && IsSendingOver;
        }

        [DataMember]
        public int RequestID
        {
            get { return _requestId; }
            set
            {
                _requestId = value;
            }
        }

        [DataMember]
        public bool FileMd5CheckResult
        {
            get { return _fileMd5CheckResult; }
            set { _fileMd5CheckResult = value; }
        }

        [DataMember]
        public string ErrMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }

        [DataMember]
        public bool IsError
        {
            get { return _isError; }
            set { _isError = value; }
        }
    }
}
