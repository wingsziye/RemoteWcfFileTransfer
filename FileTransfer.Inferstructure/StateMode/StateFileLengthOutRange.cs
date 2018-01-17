using System;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class StateFileLengthOutRange : StateBase
    {
        public override FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context)
        {
            //将文件削减至规定长度再重新进行MD5校验
            var fs = request.WorkingStream;
            if (fs.CanWrite)
            {
                fs.SetLength(request.FileRequest.FileSize);
                fs.Flush();
                context.State = new StateFileLengthEqual();
            }
            else
            {
                throw new NotImplementedException("FileStream不允许写入");
            }
            
            return context.Request(request);
        }
    }
}
