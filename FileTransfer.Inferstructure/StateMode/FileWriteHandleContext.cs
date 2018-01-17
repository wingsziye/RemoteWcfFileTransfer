using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class FileWriteHandleContext
    {
        private StateBase _state;

        public FileWriteHandleContext(StateBase state)
        {
            this._state = state;
        }

        public StateBase State
        {
            get { return _state; }
            set { _state = value; }
        }

        public FileTransferResponsed Request(ContextRequest request)
        {
            return _state.Handle(request, this);
        }
    }
}
