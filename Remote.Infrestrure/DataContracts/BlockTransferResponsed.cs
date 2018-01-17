using System.Runtime.Serialization;

namespace Remote.Infrastructure.DataContracts
{
    [DataContract]
    public class BlockTransferResponsed
    {
        private bool isError;

        [DataMember]
        public bool IsError
        {
            get { return isError; }
            set { isError = value; }
        }
    }
}
