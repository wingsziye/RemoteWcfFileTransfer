using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.StateMode
{
    internal abstract class StateBase
    {
        public abstract FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context);
    }
}
