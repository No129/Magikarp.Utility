using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Magikarp.Utility
{
    /// <summary>
    /// 提供保防功能。
    /// </summary>
    /// <remarks>
    /// Author: 黃竣祥
    /// Version: [Version]
    /// </remarks>
    public class SecurityHelper
    {
        #region -- 靜態方法 (Shared Method ) --

        /// <summary>
        /// 取得檔案雜湊值。
        /// </summary>
        /// <param name="pi_sFileFullPath">待轉換檔案完整路徑。</param>
        /// <returns>檔案雜湊值。</returns>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: [Time]
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        public static string GetHashCode(string pi_sFileFullPath)
        {
            string sReturn = string.Empty;

            //若檔案不存在則離開
            if (File.Exists(pi_sFileFullPath))
            {
                //1.選擇加密類型
                string myHashName = "SHA1";
                //2.建立HashAlgorithm類別
                using (HashAlgorithm ha = HashAlgorithm.Create(myHashName))
                {
                    //3.開啟檔案
                    using (Stream myStream = new FileStream(pi_sFileFullPath, FileMode.Open))
                    {
                        //4.產生加密的Code
                        byte[] myHash = ha.ComputeHash(myStream);
                        //5.取得雜湊值                       
                        //依檔案建立空字串
                        StringBuilder NewHashCode = new StringBuilder(myHash.Length);
                        //轉換成加密的Code
                        foreach (byte AddByte in myHash)
                        {
                            NewHashCode.AppendFormat("{0:X2}", AddByte);
                        }
                        sReturn = NewHashCode.ToString();
                    }
                }
            }

            return sReturn;
        }

        #endregion        
    }
}
