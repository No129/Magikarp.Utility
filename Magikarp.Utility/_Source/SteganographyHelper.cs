using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magikarp.Utility
{
    /// <summary>
    /// 取材自：https://www.codeproject.com/Tips/635715/Steganography-Simple-Implementation-in-Csharp
    /// 調整為輸出入為 byte[] 物件。
    /// </summary>
    /// <remarks>
    /// Author: 黃竣祥
    /// Version: [Version]
    /// </remarks>
    public class SteganographyHelper
    {

        #region -- 變數宣告 ( Declarations ) --   

        private enum State
        {
            Hiding,
            Filling_With_Zeros
        };

        #endregion

        #region -- 靜態方法 (Shared Method ) --

        /// <summary>
        /// 轉換 byte[] 為 string 型態。
        /// </summary>
        /// <param name="pi_objSource"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] pi_objSource)
        {
            return System.Text.Encoding.GetEncoding(950).GetString(pi_objSource);
        }

        /// <summary>
        /// 轉換 string 為 byte[] 型態。
        /// </summary>
        /// <param name="pi_sSource"></param>
        /// <returns></returns>
        public static Byte[] StringToByte(string pi_sSource)
        {
            return System.Text.Encoding.GetEncoding(950).GetBytes(pi_sSource);
        }

        public static Bitmap EmbedInfo(byte[] pi_objText, Bitmap pi_objTargetBitmap)
        {
            State state = State.Hiding;// initially, we'll be hiding characters in the image                       
            int charIndex = 0;// holds the index of the character that is being hidden                        
            int charValue = 0;// holds the value of the character converted to integer                        
            long pixelElementIndex = 0;// holds the index of the color element (R or G or B) that is currently being processed                       
            int zeros = 0; // holds the number of trailing zeros that have been added when finishing the process                       
            int R = 0, G = 0, B = 0; // hold pixel elements                       

            // pass through the rows
            for (int nHeightIndex = 0; nHeightIndex < pi_objTargetBitmap.Height; nHeightIndex++)
            {
                // pass through each row
                for (int nWidthIndex = 0; nWidthIndex < pi_objTargetBitmap.Width; nWidthIndex++)
                {
                    // holds the pixel that is currently being processed
                    Color pixel = pi_objTargetBitmap.GetPixel(nWidthIndex, nHeightIndex);

                    // now, clear the least significant bit (LSB) from each pixel element
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        // check if new 8 bits has been processed
                        if (pixelElementIndex % 8 == 0)
                        {
                            // check if the whole process has finished
                            // we can say that it's finished when 8 zeros are added
                            if (state == State.Filling_With_Zeros && zeros == 8)
                            {
                                // apply the last pixel on the image
                                // even if only a part of its elements have been affected
                                if ((pixelElementIndex - 1) % 3 < 2)
                                {
                                    pi_objTargetBitmap.SetPixel(nWidthIndex, nHeightIndex, Color.FromArgb(R, G, B));
                                }

                                // return the bitmap with the text hidden in
                                return pi_objTargetBitmap;
                            }

                            // check if all characters has been hidden
                            if (charIndex >= pi_objText.Length)
                            {
                                // start adding zeros to mark the end of the text
                                state = State.Filling_With_Zeros;
                            }
                            else
                            {
                                // move to the next character and process again
                                charValue = pi_objText[charIndex++];
                            }
                        }

                        // check which pixel element has the turn to hide a bit in its LSB
                        switch (pixelElementIndex % 3)
                        {
                            case 0:
                                {
                                    if (state == State.Hiding)
                                    {
                                        // the rightmost bit in the character will be (charValue % 2)
                                        // to put this value instead of the LSB of the pixel element
                                        // just add it to it
                                        // recall that the LSB of the pixel element had been cleared
                                        // before this operation
                                        R += charValue % 2;

                                        // removes the added rightmost bit of the character
                                        // such that next time we can reach the next one
                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (state == State.Hiding)
                                    {
                                        G += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (state == State.Hiding)
                                    {
                                        B += charValue % 2;

                                        charValue /= 2;
                                    }

                                    pi_objTargetBitmap.SetPixel(nWidthIndex, nHeightIndex, Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        pixelElementIndex++;

                        if (state == State.Filling_With_Zeros)
                        {
                            // increment the value of zeros until it is 8
                            zeros++;
                        }
                    }
                }
            }

            return pi_objTargetBitmap;
        }

        public static byte[] ExtractInfo(Bitmap bmp)
        {
            int colorUnitIndex = 0;
            int charValue = 0;
            byte[] objReturn = new byte[] { };

            // holds the text that will be extracted from the image
            string extractedText = String.Empty;

            // pass through the rows
            for (int nHeighIndex = 0; nHeighIndex < bmp.Height; nHeighIndex++)
            {
                // pass through each row
                for (int nWidthIndex = 0; nWidthIndex < bmp.Width; nWidthIndex++)
                {
                    Color pixel = bmp.GetPixel(nWidthIndex, nHeighIndex);

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    // get the LSB from the pixel element (will be pixel.R % 2)
                                    // then add one bit to the right of the current character
                                    // this can be done by (charValue = charValue * 2)
                                    // replace the added bit (which value is by default 0) with
                                    // the LSB of the pixel element, simply by addition
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        // if 8 bits has been added,
                        // then add the current character to the result text
                        if (colorUnitIndex % 8 == 0)
                        {
                            // reverse? of course, since each time the process occurs
                            // on the right (for simplicity)
                            charValue = SteganographyHelper.reverseBits(charValue);

                            // can only be 0 if it is the stop character (the 8 zeros)
                            if (charValue == 0)
                            {
                                return objReturn;
                            }

                            // add the current character to the result text
                            Array.Resize(ref objReturn, objReturn.Length + 1);
                            objReturn[objReturn.Length] = (byte)charValue;
                        }
                    }
                }
            }

            return objReturn;
        }

        #endregion

        #region -- 私有函式 ( Private Method) --

        private static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }

        #endregion

    }
}
