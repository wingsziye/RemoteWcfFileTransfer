using System;
using System.Collections.Generic;
using FileTransfer.Infrastructure;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;
using Remote.Infrastructure.PublicEvents;
using Remote.Infrastructure.PublicModels;

namespace Client2Server.Interfaces
{
    public class FileUpdateService : IFileUpdateService, IDisposable, IHandleFileReceiveProgress
    {
        private readonly IFileWriter _fileWriter;

        public FileUpdateService()
        {
            _fileWriter = new LocalFileWriter(this);
            ProgressMessageDic = new Dictionary<string, ProgressMessage>();
        }

        public FileTransferResponsed UpdateFile(FileTransferRequest transferData)
        {
            return _fileWriter.WriteFile(transferData);
        }

        public BlockTransferResponsed UpdateFileBlockCheckMessage(BlockTransferRequest blockMessage)
        {
            return _fileWriter.CheckBlockMessage(blockMessage);
        }

        public void Dispose()
        {
            _fileWriter.Dispose();
        }

        public Dictionary<string, ProgressMessage> ProgressMessageDic { get; set; }
        public void OnReceiveStart(string progressKey, ProgressMessage progress)
        {
            ProgressMessageDic?.Add(progressKey, progress);
        }

        public void OnRecieving(string progressKey)
        {
            try
            {
                if (ProgressMessageDic?.ContainsKey(progressKey)==true)
                {
                    PubSubEvents.Singleton.Publish(new FileReceiveProgressChangedEvent() { ProgressMsg = ProgressMessageDic[progressKey] });
                }
            }
            catch
            {
                // ignored
            }
        }

        public void OnReceiveEnd(string progressKey, bool state)
        {
            try
            {
                if (ProgressMessageDic?.ContainsKey(progressKey) == true)
                {
                    var progress = ProgressMessageDic[progressKey];
                    PubSubEvents.Singleton.Publish(
                        new FileReceiveProgressCompleteEvent() { FileName = progress.Title, IsChecked = state });
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
