using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicModels;
using Remote.Infrastructure.Tookies;

namespace FileTransfer.Infrastructure
{
    public class LocalFileReader : IDisposable
    {
        public LocalFileReader(IFileSender sendAdapter, IHandleUpdateResponsed responsedHandler=null,
            IHandleFileSendProgress progressHandler=null) : this()
        {
            this._sendAdapter = sendAdapter;
            this._responsedHandler = responsedHandler;
            this._progressHandler = progressHandler;
        }

        private LocalFileReader()
        {
            _fileStreamDic = new Dictionary<string, FileStream>();
        }

        private IFileSender _sendAdapter;
        private IHandleUpdateResponsed _responsedHandler;
        private IHandleFileSendProgress _progressHandler;
        private Dictionary<string, FileStream> _fileStreamDic;

        public void FileWait(string path)
        {
#if DEBUG
            Console.WriteLine("Waiting file release");
#endif
            bool usingWait = true;
            int times = 0;
            while (usingWait && times < 10)//最多等待10秒
            {
                usingWait = FileTookit.CheckFileIsNotUsing(path);
                if (!usingWait)
                {
                    break;
                }
                Thread.Sleep(1000);
                times++;
            }
            if (!usingWait)//仍然被占用
            {
                throw new NotSupportedException($"{path} 文件被占用");
            }
        }

        public FileBlockInfo ClacFileBlockInfo(string path,FileStream fs, int blockSize = 8192)
        {
            long fileLength = fs.Length;
            if (fileLength <= 0)
            {
                throw new ArgumentOutOfRangeException("文件长度不应为0");
            }
            long position = fs.Position;
            long blockCountF = fileLength / blockSize;
            int lastBlockSize = (int)(fileLength - blockCountF * blockSize);
            var blockCount = lastBlockSize <= 0 ? (int)blockCountF : (int)blockCountF + 1;
            if (fileLength < blockSize)
            {
                blockCount = 1;
                lastBlockSize = (int)fileLength;
                blockSize = lastBlockSize;
            }
            var info = new FileBlockInfo()
            {
                BlockCount = blockCount,
                BlockSize = blockSize,
                LastBlockSize = lastBlockSize,
                FileName = Path.GetFileName(path),
                FileMd5 = Md5.GetMd5WithFileStream(fs, fs.Position),
                FileLength = fileLength
            };
            fs.Position = position;
            return info;
        }

        protected FileBlockInfo ClacFileBlockInfo(string path, int blockSize = 8192)
        {
            var fs = _fileStreamDic[path];
            return ClacFileBlockInfo(path, fs, blockSize);
        }

        public FileTransferRequest GenerateTransferRequest(FileBlockInfo info,int randomSeed)
        {
            var request = new FileTransferRequest()
            {
                BlockIndex = 0,
                BlockCount = info.BlockCount,
                LastBlockSize = info.LastBlockSize,
                EachBlockSize = info.BlockSize,
                FileMd5 = info.FileMd5,
                FileName = info.FileName,
                FileSuffix = Path.GetExtension(info.FileName),
                FileSize = info.FileLength,
                IsSendingOver = false,
                SeekOffset = 0,
                BlockData = null,
                RequestId = randomSeed
            };
            return request;
        }

        private FileTransferRequest GenerateTransferRequest(FileBlockInfo info)
        {
            int seed = GetRandomSeed();
            var random = new Random(seed);
            return GenerateTransferRequest(info, random.Next(int.MinValue, int.MaxValue));
        }

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="blockSize"></param>
        public void RunFileTransfer(string path, int blockSize = 8192)
        {
            //prepare
            PrepareReadFile(path);
            var fileBlockInfo = ClacFileBlockInfo(path, blockSize);
            var request = GenerateTransferRequest(fileBlockInfo);
#if DEBUG
            Console.WriteLine("Preparing send file");
#endif
            //check progress handler
            ProgressMessage progress = _progressHandler?.ProgressMessageDic.ContainsKey(path)==true ? _progressHandler?.ProgressMessageDic[path] : new ProgressMessage();

            //send
            OnSendFile(path, progress, fileBlockInfo, request);

            //远程检查MD5
            request.IsSendingOver = true;
            var checkResponsed = _sendAdapter.UpdateFileData(request);

            //判断是否需要重发文件 check if need resend
            var result = ReSendFile(path, fileBlockInfo, request, checkResponsed);

            if (progress != null)
            {
                progress.StateMsg = result ? "校验成功！" : "校验失败！";
                _progressHandler?.OnSendEnd(fileBlockInfo.FileName, result);
#if DEBUG
                Console.WriteLine($"{progress.StateMsg}");
#endif
            }

            //finish send
            FinishReadFile(path);
        }

        private void OnSendFile(string path,FileStream fs, ProgressMessage progress, FileBlockInfo fileBlockInfo, FileTransferRequest request)
        {
            fs.Position = 0;
            long positionOffset = 0;
            int sendBlockSize = request.EachBlockSize;
            if (progress != null)
            {
                progress.Title = fileBlockInfo.FileName;
                progress.MaxValue = fileBlockInfo.BlockCount;
                progress.StateMsg = "Prepare OK";
                try
                {
                    _progressHandler?.OnSendStart(fileBlockInfo.FileName, progress);
#if DEBUG
                    Console.WriteLine(progress.StateMsg);
#endif
                }
                catch
                {
                    // ignored
                }
            }
            //普通循环读取和发送文件
            while (fs.Position < fs.Length)
            {
                request.BlockData = null;
                request.SeekOffset = positionOffset;

                var buffer = ReadFileBytes(path, fs.Position, sendBlockSize);
                request.BlockData = buffer;

                var responsed = _sendAdapter.UpdateFileData(request);
                _responsedHandler?.HandleResponsed(responsed);//等待外部对回应进行处理
                positionOffset = responsed.RemoteStreamPosition;

                OfflineReSendCheck(responsed, request, fileBlockInfo, fs.Position, ref positionOffset, ref sendBlockSize);//文件离线重传检查
                fs.Position = positionOffset;//进度对齐

                if (progress != null)
                {
                    progress.ProgressValue = request.BlockIndex;
                    try
                    {
                        _progressHandler?.OnSending(fileBlockInfo.FileName);
#if DEBUG
                        Console.Write($"{progress.ProgressValue}/{progress.MaxValue}");
                        Console.SetCursorPosition(0, Console.CursorTop);
                        ClearCurrentConsoleLine();
#endif
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

#if DEBUG
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
#endif

        /// <summary>
        /// 传输文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <param name="fileBlockInfo"></param>
        /// <param name="request"></param>
        private void OnSendFile(string path, ProgressMessage progress, FileBlockInfo fileBlockInfo, FileTransferRequest request)
        {
            var fs = _fileStreamDic[path];
            OnSendFile(path, fs, progress, fileBlockInfo, request);
        }

        /// <summary>
        /// 根据反馈结果，重发文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fs"></param>
        /// <param name="fileBlockInfo"></param>
        /// <param name="request"></param>
        /// <param name="checkResponsed"></param>
        public bool ReSendFile(string path, FileStream fs,FileBlockInfo fileBlockInfo, FileTransferRequest request, FileTransferResponsed checkResponsed)
        {
            int resendCountOut = 0;
            while (!checkResponsed.FileMd5CheckResult && resendCountOut < 3)//最多重发三次
            {
#if DEBUG
                Console.WriteLine("ReSending File, count "+ resendCountOut);
#endif
                //MD5检查失败重发
                var blockMsgList = ClacFileEachBlockMd5(fileBlockInfo, fs);

                if (blockMsgList.Count != fileBlockInfo.BlockCount)
                {
                    throw new Exception("校准MD5时，Block数量计算错误!");
                }
                int size = request.EachBlockSize;
                request.IsSendingOver = false;
                for (int i = 0; i < blockMsgList.Count; i++)
                {
                    var blockCheck = _sendAdapter.UpdateFileBlockMessage(blockMsgList[i]);
                    if (blockCheck.IsError)
                    {
                        if (i == blockMsgList.Count - 1)
                        {
                            size = request.LastBlockSize;
                        }
                        request.BlockIndex = i;
                        request.SeekOffset = i * fileBlockInfo.BlockSize;
                        request.BlockData = ReadFileBytes(path, request.SeekOffset, size);
                        _sendAdapter.UpdateFileData(request);
                    }
                }
                request.IsSendingOver = true;
                checkResponsed = _sendAdapter.UpdateFileData(request);
                resendCountOut++;
            }
            return checkResponsed.FileMd5CheckResult;
        }

        /// <summary>
        /// 根据反馈，重发文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileBlockInfo"></param>
        /// <param name="request"></param>
        /// <param name="checkResponsed"></param>
        private bool ReSendFile(string path, FileBlockInfo fileBlockInfo, FileTransferRequest request, FileTransferResponsed checkResponsed)
        {
            return ReSendFile(path, _fileStreamDic[path], fileBlockInfo, request, checkResponsed);
        }

        /// <summary>
        /// 计算每个文件块的MD5值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        private List<BlockTransferRequest> ClacFileEachBlockMd5(FileBlockInfo info, FileStream fs)
        {
            var position = fs.Position;
            var list = new List<BlockTransferRequest>();
            int sendBlockSize = info.BlockSize;
            int blockIndex = 0;

            while (fs.Position < fs.Length)
            {
                var r = new BlockTransferRequest()
                {
                    FileName = info.FileName,
                    BlockIndex = blockIndex,
                    BlockSize = sendBlockSize,
                    SeekOffset = (int)fs.Position
                };
                byte[] buffer = new byte[sendBlockSize];
                fs.Read(buffer, 0, buffer.Length);//read之后，Position会变,所以要先生成对象

                r.BlockMd5 = Md5.GetMd5WithBytes(buffer);
                list.Add(r);

                blockIndex++;//增加索引序号,别漏了
                if (sendBlockSize > fs.Length - fs.Position)
                {
                    sendBlockSize = (int)(fs.Length - fs.Position);
                }
            }
            fs.Position = position;
            return list;
        }

        /// <summary>
        /// 检查是否处于断点续传状态
        /// </summary>
        /// <param name="responsed"></param>
        /// <param name="request"></param>
        /// <param name="info"></param>
        /// <param name="fsPosition"></param>
        /// <param name="positionOffset"></param>
        /// <param name="sendBlockSize"></param>
        private void OfflineReSendCheck(FileTransferResponsed responsed, FileTransferRequest request, FileBlockInfo info, long fsPosition, ref long positionOffset, ref int sendBlockSize)
        {
            if (responsed.RemoteStreamPosition == fsPosition)
            {
                request.BlockIndex++;
            }
            else
            {
                int rBlockIndex = 0;
                long rPosition = 0;
                ClacBlockIndex(info, responsed.RemoteStreamPosition, out rBlockIndex, out rPosition);
                request.BlockIndex = rBlockIndex;
                positionOffset = rPosition;
            }

            if (info.BlockSize > info.FileLength - positionOffset)
            {
                sendBlockSize = (int)(info.FileLength - positionOffset);
            }
        }

        /// <summary>
        /// 计算文件块索引
        /// </summary>
        /// <param name="info"></param>
        /// <param name="remotePosition"></param>
        /// <param name="realBlockIndex"></param>
        /// <param name="realFsPosition"></param>
        private void ClacBlockIndex(FileBlockInfo info, long remotePosition, out int realBlockIndex, out long realFsPosition)
        {
            var realIndex = (int)(remotePosition / info.BlockSize);
            var realPosition = info.BlockSize * realIndex;
            if (realPosition + info.LastBlockSize == info.FileLength)//判断是否为倒数第二个Block
            {
                realBlockIndex = info.BlockCount - 1;
                realFsPosition = info.FileLength - info.LastBlockSize;
            }
            else if (remotePosition == info.FileLength)//判断是否为最后一个Block
            {
                realBlockIndex = info.BlockCount;
                realFsPosition = info.FileLength;
            }
            else
            {
                realBlockIndex = realPosition;
                realFsPosition = realIndex;
            }
        }

        /// <summary>
        /// 准备读文件
        /// </summary>
        /// <param name="path"></param>
        private void PrepareReadFile(string path)
        {
            FileWait(path);//检查文件是否被占用
            _fileStreamDic.Add(path, new FileStream(path, FileMode.Open));
            try
            {
                _progressHandler?.ProgressMessageDic.Add(path, new ProgressMessage());
            }
            catch
            {
                // ignored
            }
        }

        private byte[] ReadFileBytes(string path, long position, int readSize)
        {
            var fs = _fileStreamDic[path];
            fs.Position = position;

            byte[] buffer = new byte[readSize];

            fs.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 结束读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        private void FinishReadFile(string path)
        {
            _fileStreamDic[path].Close();
            _fileStreamDic.Remove(path);
#if DEBUG
            Console.WriteLine($"Send {path} finish and Success");
#endif
        }

        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public void Dispose()
        {
            this._fileStreamDic.Clear();
        }
    }
}
