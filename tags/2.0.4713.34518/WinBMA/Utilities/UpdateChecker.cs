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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace WinBMA.Utilities
{
    public static class UpdateChecker
    {
        public static bool IsUpdateAvailable
        {
            get
            {
                if (LatestVersion == null)
                    return false;

                return LatestVersion > App.Version;
            }
        }

        private static Version _latest;

        public static Version LatestVersion
        {
            get
            {
                if (_latest == null)
                {
                    _latest = RetrieveLatestVersionNumber();
                    Settings.SettingsDatabase.LastUpdateCheck = DateTime.Now;
                }

                return _latest;
            }
        }

        private static string URL_HOME = "http://code.google.com/p/winbma/";

        public static void OpenBrowserToHomePage()
        {
            System.Diagnostics.Process.Start(URL_HOME);
        }

        private static Version RetrieveLatestVersionNumber()
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(URL_HOME) as HttpWebRequest;
                request.Timeout = 5000;
                request.UserAgent = "WinBMA/" + App.Version.ToString();
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string pageSource = reader.ReadToEnd();

                        Regex rxSource = new Regex("\\(<span>([0-9]+.[0-9]+.[0-9]+.[0-9]+)</span>\\)", RegexOptions.IgnoreCase);
                        Match match = rxSource.Match(pageSource);

                        if (match.Success)
                        {
                            return Version.Parse(match.Groups[1].Value);
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}