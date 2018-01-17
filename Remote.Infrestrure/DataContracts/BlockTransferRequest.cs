using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class BlockTransferRequest
    {
        private string _blockMd5;
        private int _blockIndex;
        private int _blockSize;
        private int _seekOffset;
        private string fileName;

        [DataMember]
        public string BlockMd5
        {
            get { return _blockMd5; }
            set { _blockMd5 = value; }
        }

        [DataMember]
        public int BlockIndex
        {
            get { return _blockIndex; }
            set { _blockIndex = value; }
        }

        [DataMember]
        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        [DataMember]
        public int SeekOffset
        {
            get { return _seekOffset; }
            set { _seekOffset = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
    }
}
