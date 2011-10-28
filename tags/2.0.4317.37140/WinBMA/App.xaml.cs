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
using System.Windows;

namespace WinBMA
{
    /// </summary>
    public partial class App : Application
    {
        public static string AppPath
        {
            get { return System.IO.Path.GetDirectoryName(App.ExePath); }
        }

        private static DateTime _buildDate = DateTime.MinValue;

        public static DateTime BuildDate
        {
            get
            {
                if (_buildDate > DateTime.MinValue)
                    return _buildDate;

                Version assemblyVersion = App.Version;

                if (!(assemblyVersion.Build < 730) && !(assemblyVersion.Revision == 0))
                {
                    _buildDate = new DateTime(2000, 1, 1, 0, 0, 0).AddDays(assemblyVersion.Build).AddSeconds(assemblyVersion.Revision * 2);

                    if (TimeZone.IsDaylightSavingTime(_buildDate, TimeZone.CurrentTimeZone.GetDaylightChanges(_buildDate.Year)))
                    {
                        _buildDate = _buildDate.AddHours(1);
                    }

                    if (_buildDate > DateTime.Now.AddMinutes(2) | _buildDate < new DateTime(2000, 1, 1, 0, 0, 0))
                    {
                        _buildDate = DateTime.MinValue;
                    }
                }

                if (_buildDate == DateTime.MinValue)
                {
                    try
                    {
                        _buildDate = System.IO.File.GetLastWriteTime(App.ExePath);
                    }
                    catch (Exception)
                    {
                        _buildDate = DateTime.MaxValue;
                    }
                }

                return _buildDate;
            }
        }

        public static string ExePath
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().Location; }
        }

        public static bool InIDE
        {
            get { return System.Diagnostics.Debugger.IsAttached; }
        }

        public static UI.MainWindow MainAppWindow
        {
            get
            {
                if (_mainWindow == null)
                    _mainWindow = new UI.MainWindow();

                return _mainWindow;
            }
        }

        private static UI.MainWindow _mainWindow;

        public static Version Version
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.SettingsDatabase.Save();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings.SettingsDatabase.Load();

            MainAppWindow.Topmost = Settings.SettingsDatabase.AlwaysOnTop;
            MainAppWindow.Show();
        }
    }
}