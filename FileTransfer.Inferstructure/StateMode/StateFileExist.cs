using System.IO;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class StateFileExist : StateBase
    {
        public override FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context)
        {
            long fsLength;
            FileTransferResponsed responsed;

            using (request.WorkingStream = File.Open(request.WorkingPath, FileMode.Open, FileAccess.ReadWrite))
            {
                fsLength = request.WorkingStream.Length;
                if (fsLength > request.FileRequest.FileSize)
                {
                    context.State = new StateFileLengthOutRange();
                    responsed = context.Request(request);
                }
                else if (fsLength == request.FileRequest.FileSize)
                {
                    context.State = new StateFileLengthEqual();
                    responsed = context.Request(request);
                }
                else//文件实际长度既不大于也不等于远程端文件长度时
                {
                    context.State = new StateFileNormalTransfer();//若检查全部通过，则下一个状态是普通传输文件状态

                    if (fsLength >= request.FileRequest.EachBlockSize)//返回文件在倒数第2个Block时的offset,让远程端从该处发送对应Block数据,然后进入普通文件传输状态
                    {
                        var fRequest = request.FileRequest;
                        long position = ((fsLength / fRequest.EachBlockSize) - 1) * fRequest.EachBlockSize;

                        responsed = new FileTransferResponsed(request.FileRequest);
                        responsed.RemoteStreamPosition = position;
                    }
                    else//若文件长度小于区块长度，则直接按普通方式写入文件，此时远程SeekOffset应为0
                    {
                        request.WorkingStream.Close();//防止正常写入文件时触发文件被占用异常
                        responsed = context.Request(request);
                    }
                }
            }
            return responsed;//保证FileStream被Using释放后才return
        }
    }
}
