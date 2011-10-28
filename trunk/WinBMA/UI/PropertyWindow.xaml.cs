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
    public partial class PropertyWindow : Window
    {
        private DispatcherTimer authTimer;

        public PropertyWindow()
        {
            InitializeComponent();

            PROGRESS_AuthCode.Maximum = AuthAPI.BlizzardAPI.HOTP_PERIOD_LENGTH - 1;

            TEXT_FriendlyName.Text = Settings.SettingsDatabase.SelectedAuthenticator.Name;
            TEXT_FriendlyName.Watermark = String.Format("({0})", Settings.SettingsDatabase.SelectedAuthenticator.Serial);

            TEXT_Serial.Text = Settings.SettingsDatabase.SelectedAuthenticator.Serial;

            if (Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                TEXT_TokenHex.IsEnabled = true;
                TEXT_TokenHex.Text = BitConverter.ToString(Settings.SettingsDatabase.SelectedAuthenticator.DecryptedToken).Replace("-", "").ToLowerInvariant();

                TEXT_TokenBase64.IsEnabled = true;
                TEXT_TokenBase64.Text = Convert.ToBase64String(Settings.SettingsDatabase.SelectedAuthenticator.DecryptedToken);

                TEXTBLOCK_TokenInspector.Text = Utilities.HexInspector.ToHexInspectorString(Settings.SettingsDatabase.SelectedAuthenticator.DecryptedToken, 4);
            }
            else
            {
                TEXT_TokenHex.IsEnabled = false;
                TEXT_TokenHex.Text = String.Empty;

                TEXT_TokenBase64.IsEnabled = false;
                TEXT_TokenBase64.Text = String.Empty;

                TEXTBLOCK_TokenInspector.Text = String.Empty;
            }

            authTimer = new DispatcherTimer();
            authTimer.Interval = TimeSpan.FromMilliseconds(100);
            authTimer.Tick += new EventHandler(authTimer_Tick);

            this.RefreshAuthenticatorData();

            BUTTON_OK.Click += new RoutedEventHandler(BUTTON_OK_Click);

            TEXT_Serial.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);
            TEXT_RestoreCode.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);
            TEXT_AuthCode.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);

            TEXT_TokenHex.PreviewMouseDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);
            TEXT_TokenBase64.PreviewMouseDown += new MouseButtonEventHandler(TEXT_PreviewMouseLeftButtonDown);

            TABCONTROL.SelectionChanged += new SelectionChangedEventHandler(TABCONTROL_SelectionChanged);
            TEXT_Delete.TextChanged += new TextChangedEventHandler(TEXT_Delete_TextChanged);

            LINK_AddHelp.Click += new RoutedEventHandler(LINK_AddHelp_Click);

            this.Closing += new System.ComponentModel.CancelEventHandler(PropertyWindow_Closing);
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

        private void BUTTON_OK_Click(object sender, RoutedEventArgs e)
        {
            if (TABCONTROL.SelectedItem == TABITEM_Delete)
            {
                int removeIndex = Settings.SettingsDatabase.SelectedAuthenticatorIndex;
                Settings.SettingsDatabase.SelectedAuthenticatorIndex = -1;

                Settings.SettingsDatabase.Authenticators.RemoveAt(removeIndex);

                Settings.SettingsDatabase.SelectedAuthenticatorIndex = 0;
            }
            else
            {
                Settings.SettingsDatabase.SelectedAuthenticator.Name = TEXT_FriendlyName.Text.Trim();
            }

            Settings.SettingsDatabase.Save();

            this.DialogResult = true;
            this.Close();
        }

        private void LINK_AddHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpAddingWindow help = new HelpAddingWindow(Settings.SettingsDatabase.SelectedAuthenticator);
            help.Topmost = this.Topmost;
            help.Owner = this;

            help.ShowDialog();
        }

        private void PropertyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (authTimer.IsEnabled)
                authTimer.Stop();
        }

        private void RefreshAuthenticatorData()
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                PROGRESS_AuthCode.Value = WinBMA.AuthAPI.BlizzardAPI.MillisecondsSincePeriod;
            }
            else
            {
                PROGRESS_AuthCode.Value = 0;
            }

            LABEL_Skew.Content = (Settings.SettingsDatabase.ServerTimeOffset / 1000F).ToString("0.000") + "s";

            if (Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                TEXT_AuthCode.IsEnabled = true;
                TEXT_AuthCode.Text = Settings.SettingsDatabase.SelectedAuthenticator.Key;

                if (Settings.SettingsDatabase.SelectedAuthenticator.IsRestoreSupported)
                {
                    STACK_RestoreCodeEncrypted.Visibility = System.Windows.Visibility.Collapsed;
                    STACK_RestoreCodeNotAvailable.Visibility = System.Windows.Visibility.Collapsed;

                    TEXT_RestoreCode.IsEnabled = true;
                    TEXT_RestoreCode.Text = Settings.SettingsDatabase.SelectedAuthenticator.RestoreCode;
                }
                else
                {
                    STACK_RestoreCodeEncrypted.Visibility = System.Windows.Visibility.Collapsed;
                    STACK_RestoreCodeNotAvailable.Visibility = System.Windows.Visibility.Visible;

                    TEXT_RestoreCode.IsEnabled = false;
                    TEXT_RestoreCode.Text = String.Empty;
                }

                if (!this.authTimer.IsEnabled)
                    this.authTimer.Start();
            }
            else
            {
                TEXT_AuthCode.IsEnabled = false;
                TEXT_AuthCode.Text = String.Empty;

                TEXT_RestoreCode.IsEnabled = false;
                TEXT_RestoreCode.Text = String.Empty;

                if (Settings.SettingsDatabase.SelectedAuthenticator.IsRestoreSupported)
                {
                    STACK_RestoreCodeEncrypted.Visibility = System.Windows.Visibility.Visible;
                    STACK_RestoreCodeNotAvailable.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    STACK_RestoreCodeEncrypted.Visibility = System.Windows.Visibility.Collapsed;
                    STACK_RestoreCodeNotAvailable.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void TABCONTROL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TABCONTROL.SelectedItem == TABITEM_Delete)
            {
                TEXT_Delete.Text = String.Empty;
                BUTTON_OK.IsEnabled = false;
            }
            else
            {
                BUTTON_OK.IsEnabled = true;
            }
        }

        private void TEXT_Delete_TextChanged(object sender, TextChangedEventArgs e)
        {
            BUTTON_OK.IsEnabled = (TEXT_Delete.Text == "DELETE");
        }

        private void TEXT_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Clipboard.SetText(textBox.Text);
        }
    }
}