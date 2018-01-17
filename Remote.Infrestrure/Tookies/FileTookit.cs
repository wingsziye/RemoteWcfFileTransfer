using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Remote.Infrastructure.Tookies
{
    public enum FileType
    {
        其他类型 = -1,
        文件夹 = 0,
        jpg = 255216,
        gif = 7173,
        bmp = 6677,
        png = 13780,
        com = 7790,
        exe = 7790,
        dll = 7790,
        rar = 8297,
        zip = 8075,
        xml = 6063,
        html = 6033,
        aspx = 239187,
        cs = 117115,
        js = 119105,
        txt = 210187,
        sql = 255254,
        bat = 64101,
        torrent = 10056,
        rdp = 255254,
        psd = 5666,
        pdf = 3780,
        chm = 7384,
        log = 70105,
        reg = 8269,
        hlp = 6395,
        doc = 208207,
        xls = 208207,
        docx = 208207,
        xlsx = 208207,
    }

    public class FileTookit
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        /// 检查文件是否没被占用
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true:没被占用.false:正被占用</returns>
        public static bool CheckFileIsNotUsing(string path)
        {
            IntPtr vHandle = _lopen(path, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                return false;
            }
            CloseHandle(vHandle);
            return true;
        }

        /// <summary>
        /// C#检测真实文件类型函数
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public static FileType CheckRealFileType(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException();
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(fileFullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (System.IO.BinaryReader r = new System.IO.BinaryReader(fs))
                {
                    string bx = string.Empty;
                    try
                    {
                        //读两个字节
                        bx = r.ReadByte().ToString();
                        bx += r.ReadByte();
                    }
                    catch
                    {
                        r.Close();
                        fs.Close();
                    }
                    #region 文件扩展名说明

                    /*文件扩展名说明
                    * 255216 jpg
                    * 208207 doc xls ppt wps
                    * 8075 docx pptx xlsx zip
                    * 5150 txt
                    * 8297 rar
                    * 7790 exe
                    * 3780 pdf      
                    * 
                    * 4946/104116 txt
                    * 7173        gif 
                    * 255216      jpg
                    * 13780       png
                    * 6677        bmp
                    * 239187      txt,aspx,asp,sql
                    * 208207      xls.doc.ppt
                    * 6063        xml
                    * 6033        htm,html
                    * 4742        js
                    * 8075        xlsx,zip,pptx,mmap,zip
                    * 8297        rar   
                    * 01          accdb,mdb
                    * 7790        exe,dll
                    * 5666        psd 
                    * 255254      rdp 
                    * 10056       bt种子 
                    * 64101       bat 
                    * 4059        sgf    
                    */

                    #endregion
                    int fileCode = int.Parse(bx);
                    FileType type = (FileType)fileCode;
                    switch ((FileType)fileCode)
                    {
                        case FileType.jpg:
                        { }
                            break;
                        case FileType.gif:
                        { }
                            break;
                        case FileType.bmp:
                        { }
                            break;
                        case FileType.png:
                        { }
                            break;
                        case FileType.com:
                        { }
                            break;
                        case FileType.rar:
                        { }
                            break;
                        case FileType.zip:
                        { }
                            break;
                        case FileType.xml:
                        { }
                            break;
                        case FileType.html:
                        { }
                            break;
                        case FileType.aspx:
                        { }
                            break;
                        case FileType.cs:
                        { }
                            break;
                        case FileType.js:
                        { }
                            break;
                        case FileType.txt:
                        { }
                            break;
                        case FileType.sql:
                        { }
                            break;
                        case FileType.bat:
                        { }
                            break;
                        case FileType.torrent:
                        { }
                            break;
                        case FileType.psd:
                        { }
                            break;
                        case FileType.pdf:
                        { }
                            break;
                        case FileType.chm:
                        { }
                            break;
                        case FileType.log:
                        { }
                            break;
                        case FileType.reg:
                        { }
                            break;
                        case FileType.hlp:
                        { }
                            break;
                        case FileType.doc:
                        { }
                            break;
                        default:
                        {
                            type = FileType.其他类型;
                        }
                            break;
                    }
                    return type;
                }
            }
        }

        public static List<string> GetChildDirectorys(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return GetChildDirectorys(new DirectoryInfo(dirPath));
            }
            else
            {
                return null;
            }
        }

        public static List<string> GetChildDirectorys(DirectoryInfo info)
        {
            List<string> nameList = new List<string>();

            var diries = info.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var item in diries)
            {
                nameList.Add(item.FullName);
            }

            return nameList;
        }

        public static List<string> GetFileListInDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return GetFileListInDirectory(new DirectoryInfo(dirPath));
            }
            else
            {
                return null;
            }
        }

        public static List<string> GetFileListInDirectory(DirectoryInfo info)
        {
            List<string> nameList = new List<string>();
            var fileNames = from fileName
                in Directory.EnumerateFiles(info.FullName, $"*.*", SearchOption.TopDirectoryOnly)
                select fileName;

            foreach (var name in fileNames)
            {
                nameList.Add(name);
            }
            return nameList;
        }

        public static string[] FileDialogSelect()
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Multiselect = true;
            fDialog.ShowDialog();
            return fDialog.FileNames;
        }
    }
}
