/*
 * Copyright (c) 2011 WinBMA/Andrew Moore
 *
 * LICENSED UNDER THE MIT LICENSE
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Text;

namespace WinBMA.AuthAPI.Security
{
    public static class RestoreCodeProvider
    {
        public static string FromBytes(byte[] digest)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = digest.Length - 10; i < digest.Length; i++)
            {
                int charCode = digest[i] & 0x1f;

                if (charCode < 10)
                {
                    charCode += 48;
                }
                else
                {
                    charCode += 55;

                    if (charCode > 72) // I
                    {
                        charCode++;
                    }
                    if (charCode > 75) // L
                    {
                        charCode++;
                    }
                    if (charCode > 78) // O
                    {
                        charCode++;
                    }
                    if (charCode > 82) // S
                    {
                        charCode++;
                    }
                }

                builder.Append((char)charCode);
            }

            return builder.ToString();
        }

        public static byte[] ToBytes(string restoreCode)
        {
            byte[] restoreCodeBytes = Encoding.UTF8.GetBytes(restoreCode);

            if (restoreCodeBytes.Length != 10)
                return null;

            for (int i = 0; i < 10; i++)
            {
                byte cur = restoreCodeBytes[i];

                if (cur > 47 && cur < 58)
                {
                    cur -= 48;
                }
                else
                {
                    byte curMod = (byte)(cur - 55);

                    if (cur > 72)
                    {
                        curMod -= 1;
                    }

                    if (cur > 75)
                    {
                        curMod -= 1;
                    }

                    if (cur > 78)
                    {
                        curMod -= 1;
                    }

                    if (cur > 82)
                    {
                        curMod -= 1;
                    }

                    cur = curMod;
                }

                restoreCodeBytes[i] = (byte)cur;
            }

            return restoreCodeBytes;
        }
    }
}