using System;
using System.Collections.Generic;
using System.IO;
using FileTransfer.Infrastructure.Interfaces;
using FileTransfer.Infrastructure.StateMode;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.Tookies;

namespace FileTransfer.Infrastructure
{
    public class LocalFileWriter : IFileWriter,IDisposable
    {
        private readonly Dictionary<int, FileWriteHandleContext> _contextDic;
        private readonly Dictionary<int, ContextRequest> _contextRequestDic;
        private readonly IHandleFileReceiveProgress _receiveProgressHandler;

        public LocalFileWriter()
        {
            _contextDic = new Dictionary<int, FileWriteHandleContext>();
            _contextRequestDic = new Dictionary<int, ContextRequest>();
        }

        public LocalFileWriter(IHandleFileReceiveProgress receiveProgressHandler) :this()
        {
            _receiveProgressHandler = receiveProgressHandler;
        }

        public FileTransferResponsed WriteFile(FileTransferRequest transferData)
        {
            var id = transferData.RequestId;
            if (!_contextDic.ContainsKey(transferData.RequestId))//若同一个服务同时上传多个文件，则创建多个维护状态模式的Context，通过RequestID识别
            {
                _contextDic.Add(id, new FileWriteHandleContext(new StateFileFirstUpdate()));

                ContextRequest contextRequest = new ContextRequest(this._receiveProgressHandler)
                {
                    WorkingPath = FileNameTools.GetDownloadingFullPath(transferData.FileName)
                };
                _contextRequestDic.Add(id, contextRequest);
            }

            _contextRequestDic[id].FileRequest = transferData;
#if DEBUG
            //Console.WriteLine($"Data ID={id}");
#endif
            var responsed = _contextDic[id].Request(_contextRequestDic[id]);
            transferData.BlockData = null;//销毁缓存
            return responsed;
        }

        public BlockTransferResponsed CheckBlockMessage(BlockTransferRequest blockMessage)
        {
            bool isError = true;
            using (var fs = new FileStream(FileNameTools.GetDownloadingFullPath(blockMessage.FileName), FileMode.Open, FileAccess.ReadWrite))
            {
                try
                {
                    byte[] buffer = new byte[blockMessage.BlockSize];
                    fs.Position = blockMessage.SeekOffset;
                    fs.Read(buffer, 0, buffer.Length);
                    string md5 = Md5.GetMd5WithBytes(buffer);
                    if (md5 == blockMessage.BlockMd5)
                    {
                        isError = false;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return new BlockTransferResponsed() { IsError = isError };
        }

        public void Dispose()
        {
            this._contextRequestDic.Clear();
            this._contextDic.Clear();
        }
    }
}
