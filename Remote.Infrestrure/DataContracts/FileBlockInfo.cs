using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class FileBlockInfo
    {
        private int _blockCount;
        private int _blockSize;
        private int _lastBlockSize;
        private string _fileName;
        private string _fileMd5;
        private long _fileLength;

        [DataMember]
        public int BlockCount
        {
            get { return _blockCount; }
            set { _blockCount = value; }
        }

        [DataMember]
        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        [DataMember]
        public int LastBlockSize
        {
            get { return _lastBlockSize; }
            set { _lastBlockSize = value; }
        }

        [DataMember]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [DataMember]
        public string FileMd5
        {
            get { return _fileMd5; }
            set { _fileMd5 = value; }
        }

        public long FileLength
        {
            get { return _fileLength; }
            set { _fileLength = value; }
        }
    }
}
