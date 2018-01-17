using System;
using System.IO;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicModels;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class StateFileFirstUpdate : StateBase
    {
        public override FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context)
        {
            var r = request.FileRequest;
            
            if (!request.ProgressDic.ContainsKey(r.FileMd5))
            {
                var progress = new ProgressMessage();
                request.ProgressDic.Add(r.FileMd5, progress);

                try
                {
                    request?.ReceiveProgressHandler?.OnReceiveStart(r.FileMd5, progress);
                }
                catch
                {
                    // ignored
                }
            }

            //若文件存在，则进入文件存在状态
            if (File.Exists(request.WorkingPath))
            {
                context.State = new StateFileExist();
                return context.Request(request);
            }

            FileTransferResponsed responsed = new FileTransferResponsed(request.FileRequest);
            try
            {
                //创建文件并写入第一块数据
                using (var fs = new FileStream(request.WorkingPath, FileMode.Create, FileAccess.Write))
                {
                    var data = request.FileRequest.BlockData;
                    fs.Write(data, 0, data.Length);
                    responsed.RemoteStreamPosition = fs.Position;
                }
                //TODO : 将Block的信息存起来，最好把每个Block的MD5值存起来，方便使用本地对比，而不是连接远程对比。或从远程下载BLock信息，再在本地读取,PS:内存也可。
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                responsed.IsError = true;
                responsed.ErrMsg = e.Message;
            }
            context.State = new StateFileNormalTransfer();
            return responsed;
        }
    }
}
