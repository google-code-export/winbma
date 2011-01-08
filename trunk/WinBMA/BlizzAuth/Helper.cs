using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WinBMA.BlizzAuth
{
    class Helper
    {
        public static HttpWebRequest CreateRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/octet-stream";
            request.Accept = "text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2";
            request.UserAgent = "Motorola RAZR v3";
            request.Timeout = 10000;

            return request;
        }

        public static long ConvertBytesToLong(byte[] bytes)
        {
            return ConvertBytesToLong(bytes, 0);
        }

        public static long ConvertBytesToLong(byte[] bytes, int offset)
        {
            long result = 0L;
            for (int k = offset; k < offset + 8; k++)
            {
                result <<= 8;
                long longTemp = bytes[k] & 0xff;
                result += longTemp;
            }

            return result;
        }

        public static string ConvertBytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hexString.AppendFormat("{0:x2}", b);
            return hexString.ToString();
        }

        public static byte[] ConvertIntArrayToBytes(int[] intArray)
        {
            byte[] bytesArray = new byte[4 * intArray.Length];

            for (int i = 0; i < intArray.Length; i++)
            {
                int j = 4 * i;
                bytesArray[j + 3] = (byte)(intArray[i] & 0xFF);
                bytesArray[j + 2] = (byte)(intArray[i] >> 8 & 0xFF);
                bytesArray[j + 1] = (byte)(intArray[i] >> 16 & 0xFF);
                bytesArray[j] = (byte)(intArray[i] >> 24 & 0xFF);
            }

            return bytesArray;
        }

        public static byte[] ConvertHexStringToBytes(string hexString)
        {
            int NumberChars = hexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i >> 1] = Convert.ToByte(hexString.Substring(i, 2), 16); // i >> 1 is the same as i / 2
            return bytes;
        }

        public static long UnixTime
        {
            get
            {
            	return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }

        private static int hexcharToNibble(char c)
        {
            if (c >= '0' && c <= '9')
                return c - 48;
            if (c >= 'a' && c <= 'f')
                return 10 + (c - 97);
            if (c >= 'A' && c <= 'F')
                return 10 + (c - 65);
            else
                return 0;
        }

        public static string AddSpacingToAuth(string auth, int spacing = 1)
        {
            string newAuth = "";

            foreach (char a in auth)
            {
                if (newAuth.Length != 0)
                {
                    for (int i = 0; i < spacing; i++)
                    {
                        newAuth += " ";
                    }
                }

                newAuth += a;
            }

            return newAuth;
        }

        private Helper() { }
    }
}
