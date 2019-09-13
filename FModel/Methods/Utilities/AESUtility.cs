﻿using System;

namespace FModel.Methods.Utilities
{
    class AESUtility
    {
        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1) { throw new Exception("The binary key cannot have an odd number of digits"); }

            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            }

            return arr;
        }

        /// <summary>
        /// For uppercase A-F letters: return hex - (hex < 58 ? 48 : 55);
        /// For lowercase a-f letters: return hex - (hex < 58 ? 48 : 87);
        /// Or the two combined, but a bit slower: return hex - (hex < 58 ? 48 : (hex < 97 ? 55 : 87));
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static int GetHexVal(char hex)
        {
            return hex - (hex < 58 ? 48 : 55);
        }
    }
}
