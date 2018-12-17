using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Sample
{
    //我的博客主页http://www.cnblogs.com/wingsziye/
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(102/101);
            Console.ReadLine();
        }
    }

    class MyClass
    {
        public void NormalFileSend()
        {
            string path = @"c:\abc.avi";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                while (fs.Position < fs.Length)
                {
                    byte[] buffer = new byte[2048];
                    fs.Read(buffer, 0, buffer.Length);
                    Send(buffer);
                }
            }
        }

        public void Send(byte[] data)
        {
            
        }
    }
}
