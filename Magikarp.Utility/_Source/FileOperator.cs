using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Magikarp.Utility
{
    /// <summary>
    /// 提供檔案操作功能。
    /// </summary>
    /// <remarks>
    /// Author: 黃竣祥
    /// Version: [Version]
    /// </remarks>
    public class FileOperator
    {

        #region -- 靜態方法 (Shared Method ) --

        public static string CompressDirectory(string pi_sDirectoryPath)
        {
            string zipedFile = string.Format("{0}.zip", pi_sDirectoryPath);

            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    FileOperator.ZipSetp(pi_sDirectoryPath, s, "");
                }
            }

            return zipedFile;
        }

        #endregion     

        #region -- 私有函式 ( Private Method) --

        /// <summary>
        /// 遞歸遍歷目錄
        /// </summary>
        /// <param name="strDirectory">要進行壓縮的文件夾</param>
        /// <param name="s">The ZipOutputStream Object.</param>
        /// <param name="parentPath">The parent path.</param>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);
            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();
                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        #endregion
        
    }
}
