using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure;
using FileTransfer.Infrastructure.Interfaces;
using Remote.Infrastructure.DataContracts;

namespace FileTransfer.Test
{
    class FileTransferTest
    {
        private LocalFileReader reader;

        public FileTransferTest(IFileSender fileSender)
        {
            reader = new LocalFileReader(fileSender);
        }

        public void RunFullTest(string path)
        {
            reader.RunFileTransfer(path);
        }
    }
}
