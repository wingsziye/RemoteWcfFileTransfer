using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.Interfaces
{
    public interface IHandleUpdateResponsed
    {
        void HandleResponsed(FileTransferResponsed responsed);
    }
}
