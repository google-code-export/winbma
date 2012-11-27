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

using System;
using System.Text;

namespace WinBMA.Utilities
{
    public static class HexInspector
    {
        private static char ToASCIIChar(byte b)
        {
            if (b < 32 || b > 126)
                return '.';

            return (char)b;
        }

        public static string ToHexInspectorString(byte[] bytes)
        {
            return ToHexInspectorString(bytes, 16);
        }

        public static string ToHexInspectorString(byte[] bytes, int columns)
        {
            if (bytes == null)
                return String.Empty;

            StringBuilder strBuilder = new StringBuilder();

            int currColumn = 0;
            int currRow = 0;

            StringBuilder hexBuilder = new StringBuilder();
            StringBuilder asciiBuilder = new StringBuilder();

            foreach (byte b in bytes)
            {
                if (currRow != 0 && currColumn == 0)
                    strBuilder.Append("\n");

                hexBuilder.AppendFormat("{0:x2} ", b);

                if (currColumn != 0)
                    asciiBuilder.Append(' ');

                asciiBuilder.Append(ToASCIIChar(b));

                currColumn++;

                if (currColumn == columns)
                {
                    strBuilder.Append(hexBuilder);
                    strBuilder.Append("   ");
                    strBuilder.Append(asciiBuilder);

                    currColumn = 0;
                    currRow++;

                    hexBuilder.Clear();
                    asciiBuilder.Clear();
                }
            }

            if (currColumn != 0)
            {
                strBuilder.Append(hexBuilder);

                for (int i = currColumn; i < columns; i++)
                {
                    strBuilder.Append("   ");
                }

                strBuilder.Append("   ");
                strBuilder.Append(asciiBuilder);
            }

            return strBuilder.ToString();
        }
    }
}