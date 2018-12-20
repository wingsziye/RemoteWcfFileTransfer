using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicEvents;
using Remote.Infrastructure.Tookies;

namespace FileTransfer.Infrastructure.StateMode
{
    internal class StateFileLengthEqual : StateBase
    {
        public override FileTransferResponsed Handle(ContextRequest request, FileWriteHandleContext context)
        {
            var r = request.FileRequest;
            
            //Console.WriteLine("StateFileLengthEqual");
            var fs = request.WorkingStream;
            string md5 = Md5.GetMd5WithFileStream(fs, fs.Position);
            var progress = request.ProgressDic[r.FileMd5];
            FileTransferResponsed responsed = new FileTransferResponsed(request.FileRequest)
            {
                IsSendingOver = true,
                FileMd5CheckResult = md5 == request.FileRequest.FileMd5
                //文件长度相等，则视为已传输完毕
            };

            if (responsed.FileMd5CheckResult)
            {
                fs.Close();
                var path = FileNameTools.GetDownloadedFullPath(request.WorkingPath);
                Console.WriteLine(path);
                File.Move(request.WorkingPath, path);

                
                progress.ProgressValue = progress.MaxValue;
                progress.StateMsg = "校验成功！";
                try
                {
                    request?.ReceiveProgressHandler?.OnReceiveEnd(r.FileMd5, true);
                    //PubSubEvents.Singleton.Publish(new FileReceiveProgressCompleteEvent() { IsChecked = true,FileName = r.FileName });
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                request?.ReceiveProgressHandler?.OnReceiveEnd(r.FileMd5, false);
                progress.StateMsg = "校验失败！正在重新检查，请耐心等待。";
                //PubSubEvents.Singleton.GetEvent<FileReceiveProgressCompleteEvent>().Publish(new FileReceiveProgressCompleteEvent() { IsChecked = false, FileName = r.FileName });

                context.State = new StateFileNormalTransfer();//等待单个Block文件写入
            }
            return responsed;
        }
    }
}
