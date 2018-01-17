using System;
using System.Collections.Generic;
using System.IO;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicModels;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class ContextRequest:IDisposable
    {
        private FileStream _workingStream;//服务端写入流时使用
        private FileTransferRequest _fileRequest;
        private Dictionary<string, ProgressMessage> _progressDic = new Dictionary<string, ProgressMessage>();
        private IHandleFileReceiveProgress _receiveProgressHandler;

        public ContextRequest(IHandleFileReceiveProgress receiveProgressHandler)
        {
            _receiveProgressHandler = receiveProgressHandler;
        }

        private string _workingPath;

        public FileStream WorkingStream
        {
            get { return _workingStream; }
            set { _workingStream = value; }
        }

        public FileTransferRequest FileRequest
        {
            get { return _fileRequest; }
            set { _fileRequest = value; }
        }

        public string WorkingPath
        {
            get { return _workingPath; }
            set { _workingPath = value; }
        }

        public Dictionary<string, ProgressMessage> ProgressDic
        {
            get { return _progressDic; }
            set { _progressDic = value; }
        }

        public IHandleFileReceiveProgress ReceiveProgressHandler
        {
            get { return _receiveProgressHandler; }
        }

        public void Dispose()
        {
            _workingStream?.Dispose();
            _fileRequest.BlockData = null;
        }
    }
}
