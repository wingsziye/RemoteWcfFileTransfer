using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Remote.Infrastructure.Tookies
{
    public class Md5
    {
        public static string GetMd5WithBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Argument buffer is Null");
            }
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string str = string.Empty;
            byte[] hashByte = md5.ComputeHash(buffer);
            str = System.BitConverter.ToString(hashByte);
            str = str.Replace("-", "");
            return str;
        }


        public static string GetMd5WithFileStream(FileStream file, long positionOffset)
        {
            if (file == null)
            {
                throw new ArgumentNullException("Argument FileStream is Null");
            }
            file.Position = 0;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string str = string.Empty;
            byte[] hashByte = md5.ComputeHash(file);
            str = System.BitConverter.ToString(hashByte);
            str = str.Replace("-", "");
            file.Position = positionOffset;
            return str;
        }

        public static string GetMd5WithFilePath(string path)
        {
            string md5 = string.Empty;
            using (var fs = File.OpenRead(path))
            {
                md5 = GetMd5WithFileStream(fs, fs.Position);
            }
            return md5;
        }

        public static async Task<string> GetMd5WithFileStreamAsync(FileStream file, long positionOffset)
        {
            string str = string.Empty;
            await Task.Run(() =>
            {
                str = GetMd5WithFileStream(file, positionOffset);
            });
            return str;
        }
    }
}
