using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class FileTransferRequest
    {
        private int _requestId;//上传方生成的随机值

        private bool _isSendingOver = false;//上传方是否全部发送完毕

        private string _fileMd5;//文件Hash

        private string _fileName;//文件名,带后缀名

        private string _fileSuffix;//文件名的后缀

        private int _eachBlockSize;//每一个区块的大小

        private int _lastBlockSize;//最后一个区块的大小

        private long _fileSize;//文件总大小

        private int _blockCount;//剩余区块个数  //(总文件大小=(总区块个数-1)*每个区块的大小+最后一个区块的大小)

        private int _blockIndex;//第几个区块

        private long _seekOffset;//要写入区块的文件Offset

        private byte[] _blockData;

        [DataMember]
        public int RequestId
        {
            get { return _requestId; }
            set { _requestId = value; }
        }

        [DataMember]
        public bool IsSendingOver
        {
            get { return _isSendingOver; }
            set { _isSendingOver = value; }
        }

        [DataMember]
        public string FileMd5
        {
            get { return _fileMd5; }
            set { _fileMd5 = value; }
        }

        [DataMember]
        public int LastBlockSize
        {
            get { return _lastBlockSize; }
            set { _lastBlockSize = value; }
        }

        [DataMember]
        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        [DataMember]
        public long SeekOffset
        {
            get { return _seekOffset; }
            set { _seekOffset = value; }
        }

        [DataMember]
        public int BlockCount
        {
            get { return _blockCount; }
            set { _blockCount = value; }
        }

        [DataMember]
        public int BlockIndex
        {
            get { return _blockIndex; }
            set { _blockIndex = value; }
        }

        [DataMember]
        public int EachBlockSize
        {
            get { return _eachBlockSize; }
            set { _eachBlockSize = value; }
        }

        [DataMember]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [DataMember]
        public string FileSuffix
        {
            get { return _fileSuffix; }
            set { _fileSuffix = value; }
        }

        [DataMember]
        public byte[] BlockData
        {
            get { return _blockData; }
            set { _blockData = value; }
        }
    }
}
