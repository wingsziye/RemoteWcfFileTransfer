using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer.Infrastructure;

namespace FileTransfer.Sample
{
    //我的博客主页http://www.cnblogs.com/wingsziye/
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\UnityProjects\VrShoot_180419.7z";
            new FileTransferTest().RunFullTest(path);
            Console.ReadLine();
        }
    }



    class LocalFileTransferMode
    {
        public void NormalFileSend(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                while (fs.Position < fs.Length)
                {
                    byte[] buffer = new byte[2048];//base on your application to decide block size
                    fs.Read(buffer, 0, buffer.Length);
                    Send(buffer);
                }
            }
        }

        public void Send(byte[] data)
        {
            //just pretend the data have send to server
        }
    }
}
