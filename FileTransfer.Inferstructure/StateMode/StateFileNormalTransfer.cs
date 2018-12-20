using System;
using System.IO;
using System.Reactive.Linq;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicEvents;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class StateFileNormalTransfer : StateBase
    {
        public override FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context)
        {
            FileTransferResponsed responsed = new FileTransferResponsed(request.FileRequest);
            var r = request.FileRequest;

            if (r.IsSendingOver)//在文件写入完成时，检查文件MD5
            {
                if (request.ProgressDic.ContainsKey(r.FileMd5))
                {
                    request.ProgressDic[r.FileMd5].StateMsg = "校验中";
                    //request.ProgressDic.Remove(r.FileMd5);
                }
#if DEBUG
                Console.WriteLine("Normal file send finished，prepare to check fileMD5");
#endif
                context.State = new StateFileExist();//检查文件长度是否相等或超过。PS:可能存在陷入无限死循环的隐患，调试时注意
                return context.Request(request);
            }

            try
            {
                using (var fs = File.OpenWrite(request.WorkingPath))
                {
                    //在文件未传输完成时，持续写入文件
                    using (BinaryWriter bs = new BinaryWriter(fs))
                    {
                        //Console.WriteLine($"seekoffset{r.SeekOffset},index={r.BlockIndex},length={r.BlockData.Length}");
                        bs.Seek((int)r.SeekOffset, SeekOrigin.Begin);
                        bs.Write(r.BlockData);
                        responsed.RemoteStreamPosition = fs.Position;
                    }
                }
                var progress = request.ProgressDic[r.FileMd5];
                progress.MaxValue = r.BlockCount;
                progress.ProgressValue = r.BlockIndex;
                progress.Title = r.FileName;
                progress.StateMsg = "传输中";

                try
                {
                    request?.ReceiveProgressHandler?.OnRecieving(r.FileMd5);
                    //PubSubEvents.Singleton.Publish(new FileReceiveProgressChangedEvent() { ProgressMsg = request.ProgressDic[r.FileMd5] });
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                responsed.IsError = true;
                responsed.ErrMsg = e.Message;
            }
            return responsed;
        }
    }
}