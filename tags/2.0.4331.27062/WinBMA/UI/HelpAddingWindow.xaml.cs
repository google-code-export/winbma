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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WinBMA.UI
{
    public partial class HelpAddingWindow : Window
    {
        AuthAPI.Authenticator auth;
        DispatcherTimer authTimer;

        public HelpAddingWindow(AuthAPI.Authenticator auth)
        {
            InitializeComponent();

            PROGRESS_AuthCode.Maximum = AuthAPI.BlizzardAPI.HOTP_PERIOD_LENGTH - 1;

            LINK_BattleNet.Click += new RoutedEventHandler(LINK_BattleNet_Click);

            this.auth = auth;

            authTimer = new DispatcherTimer();
            authTimer.Interval = TimeSpan.FromMilliseconds(100);
            authTimer.Tick += new EventHandler(authTimer_Tick);

            TEXT_Serial.Text = auth.Serial;

            this.RefreshAuthenticatorData();

            TEXT_AuthCode.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);
            TEXT_Serial.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);

            this.Closing += new System.ComponentModel.CancelEventHandler(HelpAddingWindow_Closing);
        }

        private void authTimer_Tick(object sender, EventArgs e)
        {
            long oldValue = 0;
            long currentValue = WinBMA.AuthAPI.BlizzardAPI.MillisecondsSincePeriod;

            oldValue = (long)PROGRESS_AuthCode.Value;
            PROGRESS_AuthCode.Value = currentValue;

            if (oldValue > currentValue)
            {
                this.RefreshAuthenticatorData();
            }
        }

        private void HelpAddingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (authTimer.IsEnabled)
                authTimer.Stop();
        }

        private void LINK_BattleNet_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.battle.net/bma");
        }

        private void RefreshAuthenticatorData()
        {
            LABEL_Skew.Content = (Settings.SettingsDatabase.ServerTimeOffset / 1000F).ToString("0.000") + "s";

            if (auth.IsDecrypted)
            {
                PROGRESS_AuthCode.Value = WinBMA.AuthAPI.BlizzardAPI.MillisecondsSincePeriod;
            }
            else
            {
                PROGRESS_AuthCode.Value = 0;
            }

            if (auth.IsDecrypted)
            {
                TEXT_AuthCode.Text = auth.Key;

                if (!this.authTimer.IsEnabled)
                    this.authTimer.Start();
            }
            else
            {
                TEXT_AuthCode.IsEnabled = false;
                TEXT_AuthCode.Text = String.Empty;
            }
        }

        private void TEXT_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == TEXT_Serial)
            {
                Clipboard.SetText(textBox.Text.Replace("-", ""));
            }
            else
            {
                Clipboard.SetText(textBox.Text);
            }
        }
    }
}