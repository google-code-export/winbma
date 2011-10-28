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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinBMA.UI
{
    /// </summary>
    public partial class RestoreAuthWindow : Window
    {
        private string authenticatorName;
        private BackgroundWorker bgVerifier;
        private BackgroundWorker bgWorker;
        private string checkRestoreCode;
        private string checkSerial;
        private int[] dashPositions = { 2, 7, 12 };
        private AuthAPI.Security.EncryptionProvider.EncryptionType encryptionType;
        private bool _fireRestoreCodeTextChanged = true;
        private bool _fireSerialTextChanged = true;
        private AuthAPI.Authenticator newAuthenticator;
        private string userPassword;

        public RestoreAuthWindow()
        {
            InitializeComponent();

            WizardPage2Validation();

            TEXTBLOCK_Error.Visibility = System.Windows.Visibility.Collapsed;

            TEXTBLOCK_CurrentUser.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            TEXTBLOCK_LocalMachine.Text = Environment.MachineName;

            CHECK_ProtectPassword.Checked += new RoutedEventHandler(WizardPage4_TriggerValidation);
            CHECK_ProtectPassword.Unchecked += new RoutedEventHandler(WizardPage4_TriggerValidation);

            CHECK_ProtectWindows.Checked += new RoutedEventHandler(WizardPage4_TriggerValidation);
            CHECK_ProtectWindows.Unchecked += new RoutedEventHandler(WizardPage4_TriggerValidation);

            TEXT_Password.TextChanged += new TextChangedEventHandler(WizardPage4_TriggerValidation_TextChanged);

            TEXT_RestoreSerial.TextChanged += new TextChangedEventHandler(TEXT_RestoreSerial_TextChanged);
            TEXT_RestoreCodeInput.TextChanged += new TextChangedEventHandler(TEXT_RestoreCodeInput_TextChanged);

            WIZARD.PageChanged += new RoutedEventHandler(WIZARD_PageChanged);

            bgVerifier = new BackgroundWorker();
            bgVerifier.WorkerReportsProgress = true;

            bgVerifier.ProgressChanged += new ProgressChangedEventHandler(bgVerifier_ProgressChanged);
            bgVerifier.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgVerifier_RunWorkerCompleted);
            bgVerifier.DoWork += new DoWorkEventHandler(bgVerifier_DoWork);

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;

            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);

            LINK_AddHelp.Click += new RoutedEventHandler(LINK_AddHelp_Click);
            LINK_CopyToClipboard.Click += new RoutedEventHandler(LINK_CopyToClipboard_Click);

            TEXT_Serial.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_Serial_PreviewMouseLeftButtonDown);
            TEXT_RestoreCode.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TEXT_RestoreCode_PreviewMouseLeftButtonDown);
        }

        private void bgVerifier_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bgVerifier.ReportProgress(0, "Verifying Restore Code...");

                newAuthenticator = AuthAPI.BlizzardAPI.RestoreAuthenticator(checkSerial, checkRestoreCode);

                if (newAuthenticator == null)
                {
                    e.Result = false;
                    return;
                }

                bgVerifier.ReportProgress(2, "Restore Code Verified!");

                System.Threading.Thread.Sleep(1000);

                e.Result = true;
            }
            catch (Exception)
            {
                e.Result = false;
            }
        }

        private void bgVerifier_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PROGRESS_VerifyTasks.Value = e.ProgressPercentage;
            LABEL_VerifyCurrentTask.Content = e.UserState;
        }

        private void bgVerifier_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (newAuthenticator == null)
            {
                TEXTBLOCK_Error.Visibility = System.Windows.Visibility.Visible;
                WIZARD.CurrentPage = WIZARDPAGE_RestoreCode;
            }
            else
            {
                WIZARD.CurrentPage = WIZARDPAGE_Protect;

                WIZARD.CurrentPage = WIZARDPAGE_Protect;
            }
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (encryptionType != AuthAPI.Security.EncryptionProvider.EncryptionType.None)
                {
                    bgWorker.ReportProgress(0, "Encrypting Authenticator...");
                }

                newAuthenticator.Encrypt(encryptionType, userPassword);
                newAuthenticator.Name = authenticatorName;

                bgWorker.ReportProgress(1, "Adding Authenticator to Database...");

                Settings.SettingsDatabase.Authenticators.Add(newAuthenticator);
                Settings.SettingsDatabase.Save();

                bgWorker.ReportProgress(2, "Authenticator Restored!");

                System.Threading.Thread.Sleep(1000);

                e.Result = true;
            }
            catch (Exception)
            {
                e.Result = false;
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PROGRESS_Tasks.Value = e.ProgressPercentage;
            LABEL_CurrentTask.Content = e.UserState;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (newAuthenticator != null)
            {
                TEXT_Serial.Text = newAuthenticator.Serial;
                TEXT_RestoreCode.Text = newAuthenticator.RestoreCode;
                WIZARD.CurrentPage = WIZARDPAGE_Success;
            }
            else
            {
                WIZARD.CurrentPage = WIZARDPAGE_Fail;
            }
        }

        private void LINK_AddHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpAddingWindow help = new HelpAddingWindow(newAuthenticator);
            help.Topmost = this.Topmost;
            help.Owner = this;

            help.ShowDialog();
        }

        private void LINK_CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Format("Serial: {0}\nRestore Code: {1}", newAuthenticator.Serial, newAuthenticator.RestoreCode));
        }

        private void TEXT_RestoreCode_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(newAuthenticator.RestoreCode);
        }

        private void TEXT_RestoreCodeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_fireRestoreCodeTextChanged)
                return;

            _fireRestoreCodeTextChanged = false;

            int caretIndex = TEXT_RestoreCodeInput.CaretIndex;

            TEXT_RestoreCodeInput.Text = TEXT_RestoreCodeInput.Text.ToUpperInvariant();

            TEXT_RestoreCodeInput.CaretIndex = caretIndex;

            _fireRestoreCodeTextChanged = true;

            WizardPage2Validation();
        }

        private void TEXT_RestoreSerial_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_fireSerialTextChanged)
                return;

            _fireSerialTextChanged = false;

            int caretIndex = TEXT_RestoreSerial.CaretIndex;

            foreach (int i in dashPositions)
            {
                if (TEXT_RestoreSerial.Text.Length > i && TEXT_RestoreSerial.Text[i] != '-')
                {
                    if (caretIndex > i)
                        caretIndex++;

                    TEXT_RestoreSerial.Text = TEXT_RestoreSerial.Text.Substring(0, i) + '-' + TEXT_RestoreSerial.Text.Substring(i);
                }
            }

            TEXT_RestoreSerial.Text = TEXT_RestoreSerial.Text.ToUpperInvariant();

            TEXT_RestoreSerial.CaretIndex = caretIndex;

            _fireSerialTextChanged = true;

            WizardPage2Validation();
        }

        private void TEXT_Serial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(newAuthenticator.Serial);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (bgWorker.IsBusy)
                e.Cancel = true;

            if (WIZARD.CurrentPage == WIZARDPAGE_Success)
                this.DialogResult = true;
            else
                this.DialogResult = null;
        }

        private void WIZARD_PageChanged(object sender, RoutedEventArgs e)
        {
            if (WIZARD.CurrentPage == WIZARDPAGE_SettingsSummary)
            {
                string strEncryptionType = null;

                if (CHECK_ProtectPassword.IsChecked == true)
                {
                    strEncryptionType = "Password";
                }

                if (CHECK_ProtectWindows.IsChecked == true)
                {
                    if (strEncryptionType != null)
                        strEncryptionType += "\n";
                    else
                        strEncryptionType = "";

                    strEncryptionType += (RADIO_ProtectLocalMachine.IsChecked == true)
                        ? "Locked to local machine (" + TEXTBLOCK_LocalMachine.Text + ")" : "Locked to current user (" + TEXTBLOCK_CurrentUser.Text + ")";
                }

                if (strEncryptionType == null)
                {
                    LABEL_EncryptionType.IsEnabled = false;
                    LABEL_EncryptionType.Content = "(None)";
                }
                else
                {
                    LABEL_EncryptionType.IsEnabled = true;
                    LABEL_EncryptionType.Content = strEncryptionType;
                }

                if (String.IsNullOrWhiteSpace(TEXT_FriendlyName.Text))
                {
                    LABEL_FriendlyName.IsEnabled = false;
                    LABEL_FriendlyName.Content = "(Serial will be used)";
                }
                else
                {
                    LABEL_FriendlyName.IsEnabled = true;
                    LABEL_FriendlyName.Content = TEXT_FriendlyName.Text.Trim();
                }
            }
            else if (WIZARD.CurrentPage == WIZARDPAGE_Progress)
            {
                encryptionType = AuthAPI.Security.EncryptionProvider.EncryptionType.None;

                if (CHECK_ProtectPassword.IsChecked == true)
                {
                    userPassword = TEXT_Password.Text;
                    encryptionType |= AuthAPI.Security.EncryptionProvider.EncryptionType.Password;
                }

                if (CHECK_ProtectWindows.IsChecked == true)
                {
                    encryptionType |= (RADIO_ProtectLocalMachine.IsChecked == true)
                        ? AuthAPI.Security.EncryptionProvider.EncryptionType.LocalMachine : AuthAPI.Security.EncryptionProvider.EncryptionType.LocalUser;
                }

                if (!String.IsNullOrWhiteSpace(TEXT_FriendlyName.Text))
                {
                    authenticatorName = TEXT_FriendlyName.Text.Trim();
                }

                bgWorker.RunWorkerAsync();
            }
            else if (WIZARD.CurrentPage == WIZARDPAGE_VerifyCode)
            {
                newAuthenticator = null;
                PROGRESS_VerifyTasks.Value = 0;

                checkSerial = TEXT_RestoreSerial.Text;
                checkRestoreCode = TEXT_RestoreCodeInput.Text;

                bgVerifier.RunWorkerAsync();
            }
        }

        private void WizardPage2Validation()
        {
            WIZARDPAGE_RestoreCode.CanSelectNextPage = (TEXT_RestoreCodeInput.Text.Length == 10 && AuthAPI.Authenticator.IsValidSerial(TEXT_RestoreSerial.Text));
        }

        private void WizardPage4_TriggerValidation(object sender, RoutedEventArgs e)
        {
            WizardPage4Validation();
        }

        private void WizardPage4_TriggerValidation_TextChanged(object sender, RoutedEventArgs e)
        {
            WizardPage4Validation();
        }

        private void WizardPage4Validation()
        {
            if (CHECK_ProtectPassword.IsChecked == true || CHECK_ProtectWindows.IsChecked == true)
            {
                LABEL_NoEncryptionWarning.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LABEL_NoEncryptionWarning.Visibility = System.Windows.Visibility.Visible;
                WIZARDPAGE_Protect.CanSelectNextPage = true;
            }

            if (CHECK_ProtectPassword.IsChecked == true)
            {
                WIZARDPAGE_Protect.CanSelectNextPage = TEXT_Password.Text != string.Empty;
            }
        }
    }
}