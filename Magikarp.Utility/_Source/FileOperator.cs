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
    /// Version: 20170911
    /// </remarks>
    public class FileOperator
    {

        #region -- 靜態方法 (Shared Method ) --

        /// <summary>
        /// 壓縮路徑(遞迴)為單一檔案。
        /// </summary>
        /// <param name="pi_sDirectoryPath">待壓縮路徑根目錄。</param>
        /// <returns>壓縮檔路徑。</returns>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: 2017/09/11
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        public static string CompressDirectory(string pi_sDirectoryPath)
        {
            string sReturn = string.Format("{0}.zip", pi_sDirectoryPath);

            using (System.IO.FileStream objFileStream = System.IO.File.Create(sReturn))
            {
                using (ZipOutputStream objZipOutputStream = new ZipOutputStream(objFileStream))
                {
                    FileOperator.ZipSetp(pi_sDirectoryPath, objZipOutputStream, "");
                }
            }

            return sReturn;
        }

        /// <summary>
        /// 解壓縮文件。
        /// </summary>
        /// <param name="pi_sTargetFile">目標文件。</param>
        /// <returns>解壓縮路徑。</returns>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: 2017/09/11
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        public static string Decompress(string pi_sTargetFile)
        {
            if (!File.Exists(pi_sTargetFile))
            {
                throw new System.IO.FileNotFoundException("指定要解壓縮的文件: " + pi_sTargetFile + " 不存在!");
            }

            string sReturn = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(pi_sTargetFile), System.IO.Path.GetFileNameWithoutExtension(pi_sTargetFile));
            
            if(System.IO.Directory.Exists(sReturn) == false)
            {
                Directory.CreateDirectory(sReturn);
            }            

            using (ZipInputStream objZipInputStream = new ZipInputStream(File.OpenRead(pi_sTargetFile)))
            {
                ZipEntry objZipEntry = null;

                while ((objZipEntry = objZipInputStream.GetNextEntry()) != null)
                {
                    string sEntryDirectory = Path.GetDirectoryName(objZipEntry.Name);
                    string sEntryFileName = Path.GetFileName(objZipEntry.Name);

                    if (sEntryDirectory.Length > 0)
                    {
                        Directory.CreateDirectory(string.Format("{0}\\{1}", sReturn, sEntryDirectory));
                    }

                    if (sEntryFileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(string.Format("{0}\\{1}", sReturn, objZipEntry.Name)))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];

                            while (true)
                            {
                                size = objZipInputStream.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return sReturn;
        }

        #endregion

        #region -- 私有函式 ( Private Method) --

        /// <summary>
        /// 遞迴目錄。
        /// </summary>
        /// <param name="pi_strDirectory">要進行壓縮的文件夾</param>
        /// <param name="pi_objZipOutputStream">ZipOutputStream 實體。</param>
        /// <param name="pi_sParentPath">壓縮檔內的上層路徑。</param>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: 2017/09/11
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        private static void ZipSetp(string pi_strDirectory, ZipOutputStream pi_objZipOutputStream, string pi_sParentPath)
        {
            if (pi_strDirectory[pi_strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                pi_strDirectory += Path.DirectorySeparatorChar;
            }

            string[] filenames = Directory.GetFileSystemEntries(pi_strDirectory);

            // 遍歷所有的文件和目錄。
            foreach (string sFile in filenames)
            {
                if (Directory.Exists(sFile)) // 先視為目錄處理：若存在目錄，就遞迴該目錄包含的文件及目錄。
                {
                    string sParentPath = pi_sParentPath;

                    sParentPath += sFile.Substring(sFile.LastIndexOf("\\") + 1); // 累積壓縮檔內的路徑。
                    sParentPath += "\\";
                    ZipSetp(sFile, pi_objZipOutputStream, sParentPath);
                }
                else // 否则視為文件處理。
                {                    
                    using (FileStream objFileStream = File.OpenRead(sFile)) // 打開壓縮檔案。
                    {
                        byte[] objBuffer = new byte[objFileStream.Length];
                        string sFileName = pi_sParentPath + sFile.Substring(sFile.LastIndexOf("\\") + 1);
                        ZipEntry objZipEntry = new ZipEntry(sFileName);

                        objFileStream.Read(objBuffer, 0, objBuffer.Length); //將檔案讀入暫存器 ( objBuffer )。
                        objZipEntry.DateTime = DateTime.Now;
                        objZipEntry.Size = objFileStream.Length;
                        objFileStream.Close();
                        pi_objZipOutputStream.PutNextEntry(objZipEntry);
                        pi_objZipOutputStream.Write(objBuffer, 0, objBuffer.Length); //將暫存器內容寫入壓縮檔。
                    }
                }
            }
        }

        #endregion

    }
}
