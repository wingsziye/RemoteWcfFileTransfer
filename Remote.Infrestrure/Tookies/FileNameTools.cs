using System;
using System.Configuration;
using System.IO;

namespace Remote.Infrastructure.Tookies
{
    public class FileNameTools
    {
        public const string DefaultSuffix = "WcfDownload";

        public static string GetDownloadingTempFileName(string fileName)
        {
            var setting = ConfigurationManager.AppSettings.GetValues("DownloadSuffix") ;
            string value = setting == null ? DefaultSuffix : setting[0];//若setting为空，则使用默认
            return $"{fileName}.{value}";
        }

        public static string GetTempPath(string fileName)
        {
            var setting = ConfigurationManager.AppSettings.GetValues("DownloadTempDir");
            string value = setting == null ? fileName : setting[0];//若
            return Path.GetDirectoryName(Path.GetFullPath(value));
        }

        public static string GetDownloadingFullPath(string fileName)
        {
            return Path.Combine(GetTempPath(fileName), GetDownloadingTempFileName(fileName));
        }

        public static string GetDownloadedFullPath(string fileTempName)
        {
            var setting = ConfigurationManager.AppSettings.GetValues("DownloadSuffix");
            string value = setting == null ? DefaultSuffix : setting[0];//若
            var dir = Path.GetDirectoryName(fileTempName);
            var name = Path.GetFileName(fileTempName);
            var nameTrim = Path.GetFileNameWithoutExtension(name);
            int count = 1;
            var fullPath = Path.Combine(dir, nameTrim);
            while (File.Exists(fullPath))
            {
                count++;
                var insertName = nameTrim.Insert(nameTrim.LastIndexOf("."), $"({count})");
                fullPath = Path.Combine(dir, insertName);
            }
            return fullPath;
        }
    }
}
