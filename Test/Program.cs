using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure;

namespace FileTransfer.Test
{
    //我的博客主页http://www.cnblogs.com/wingsziye/
    class Program
    {
        static void Main(string[] args)
        {
            string path = @".\zfb.jpg";
            var sender = new LocalFileSender(new LocalFileWriter());
            new FileTransferTest(sender).RunFullTest(path);
            Console.ReadLine();
        }
    }
}
