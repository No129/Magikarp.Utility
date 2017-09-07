using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Magikarp.Utility
{
    /// <summary>
    /// 提供圖片操作功能。
    /// </summary>
    /// <remarks>
    /// Author: 黃竣祥
    /// Version: [Version]
    /// </remarks>
    public class ImageHelper
    {

        #region -- 靜態方法 (Shared Method ) --

        /// <summary>
        /// 載入指定路徑圖片至記憶體。
        /// </summary>
        /// <param name="pi_sImageFullPath">圖片來源完整路徑。</param>
        /// <param name="pi_nBitmapCacheOption">快取選項。</param>
        /// <param name="pi_nBitmapCreateOption">建構選項。</param>
        /// <returns>圖片物件。</returns>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: [Time]
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        public static BitmapImage Load(string pi_sImageFullPath, BitmapCacheOption pi_nBitmapCacheOption = BitmapCacheOption.OnLoad, BitmapCreateOptions pi_nBitmapCreateOption = BitmapCreateOptions.IgnoreImageCache)
        {
            BitmapImage objReturn = new BitmapImage();

            objReturn.BeginInit();
            objReturn.UriSource = new Uri(pi_sImageFullPath);
            objReturn.CacheOption = BitmapCacheOption.OnLoad;
            objReturn.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            objReturn.EndInit();

            return objReturn;
        }

        /// <summary>
        /// 比對圖片雜湊值是否一致。
        /// </summary>
        /// <param name="pi_sFirstImageFullPath">第一張圖片的檔案完整路徑。</param>
        /// <param name="pi_sSecondImageFullP">第二張圖片的檔案完整路徑。</param>
        /// <returns>比對圖片是否一致。</returns>
        /// <remarks>
        /// Author: 黃竣祥
        /// Time: [Time]
        /// History: N/A
        /// DB Object: N/A      
        /// </remarks>
        public static bool Compare(string pi_sFirstImageFullPath, string pi_sSecondImageFullP)
        {
            Bitmap objFirstImage = new Bitmap(pi_sFirstImageFullPath);
            Bitmap objSecondImage = new Bitmap(pi_sSecondImageFullP);

            MemoryStream memoryStream = new MemoryStream();
            //將firstImage存入記憶體
            objFirstImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            //取得firstImage的Base64雜湊值
            String firstBase64 = Convert.ToBase64String(memoryStream.ToArray());

            memoryStream = new MemoryStream();
            //將secondImage存入記憶體
            objSecondImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            //取得secondImage的Base64雜湊值
            String secondBase64 = Convert.ToBase64String(memoryStream.ToArray());

            //比較firstImage與secondImage的Base64雜湊值
            if (firstBase64.Equals(secondBase64))
                return true;
            else
                return false;
        }

        #endregion        
    }
}
