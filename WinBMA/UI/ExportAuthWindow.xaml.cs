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

namespace WinBMA.UI
{
    public partial class ExportAuthWindow : Window
    {
        private string authenticatorName;
        private BackgroundWorker bgWorker;
        private AuthAPI.Security.EncryptionProvider.EncryptionType encryptionType;
        private string savePath;
        private string userPassword;

        public ExportAuthWindow()
        {
            InitializeComponent();

            CHECK_ProtectPassword.Checked += new RoutedEventHandler(WizardPage2_TriggerValidation);
            CHECK_ProtectPassword.Unchecked += new RoutedEventHandler(WizardPage2_TriggerValidation);

            TEXT_Password.TextChanged += new TextChangedEventHandler(WizardPage2_TriggerValidation_TextChanged);

            WIZARD.PageChanged += new RoutedEventHandler(WIZARD_PageChanged);

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;

            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bgWorker.ReportProgress(1, "Requesting Authenticator Token...");

                AuthAPI.Authenticator newAuthenticator = Settings.SettingsDatabase.SelectedAuthenticator.Clone() as AuthAPI.Authenticator;

                newAuthenticator.Name = authenticatorName;

                if (encryptionType != AuthAPI.Security.EncryptionProvider.EncryptionType.None)
                {
                    bgWorker.ReportProgress(1, "Encrypting Authenticator...");

                    newAuthenticator.Encrypt(encryptionType, userPassword);
                }

                bgWorker.ReportProgress(2, "Saving Authenticator to File...");

                newAuthenticator.ToFile(savePath);

                bgWorker.ReportProgress(3, "Authenticator Exported!");

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
            if (((bool)e.Result) == true)
            {
                WIZARD.CurrentPage = WIZARDPAGE_Success;
            }
            else
            {
                WIZARD.CurrentPage = WIZARDPAGE_Fail;
            }
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
                bgWorker.ReportProgress(0, "Selecting File...");
                encryptionType = AuthAPI.Security.EncryptionProvider.EncryptionType.None;

                if (CHECK_ProtectPassword.IsChecked == true)
                {
                    userPassword = TEXT_Password.Text;
                    encryptionType |= AuthAPI.Security.EncryptionProvider.EncryptionType.Password;
                }

                if (!String.IsNullOrWhiteSpace(TEXT_FriendlyName.Text))
                {
                    authenticatorName = TEXT_FriendlyName.Text.Trim();
                }

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Title = "Export Authenticator";
                dlg.FileName = Settings.SettingsDatabase.SelectedAuthenticator.Serial + ".bma";
                dlg.DefaultExt = ".bma"; // Default file extension
                dlg.AddExtension = true;
                dlg.OverwritePrompt = true;
                dlg.Filter = "WinBMA Exported Authenticator (.bma)|*.bma"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    savePath = dlg.FileName;
                    bgWorker.RunWorkerAsync();
                }
                else
                {
                    this.Close();
                    return;
                }
            }
        }

        private void WizardPage2_TriggerValidation(object sender, RoutedEventArgs e)
        {
            WizardPage2Validation();
        }

        private void WizardPage2_TriggerValidation_TextChanged(object sender, RoutedEventArgs e)
        {
            WizardPage2Validation();
        }

        private void WizardPage2Validation()
        {
            if (CHECK_ProtectPassword.IsChecked == true)
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