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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WinBMA.Utilities;

namespace WinBMA.UI.Controls
{
    public partial class AuthenticatorPanel : UserControl
    {
        private static Dictionary<string, string> _availableThemes;

        public static Dictionary<string, string> AvailableThemes
        {
            get
            {
                if (_availableThemes == null)
                {
                    _availableThemes = new Dictionary<string, string>();
                    _availableThemes.Add("BattleNet", "Battle.net");
                    _availableThemes.Add("BlizzardMobile", "Mobile Authenticator");
                    _availableThemes.Add("ClassicWinBMA", "Classic");
                }

                return _availableThemes;
            }
        }

        private Utilities.SystemHotKey HOTKEY_system;
        private bool _ignoreIndexChange = false;
        private Button PART_Btn_Code;
        private Button PART_Btn_ContextMenuTarget;
        private ContentControl PART_CanonicalName;
        private ComboBox PART_Cmb_AuthList;
        private ContextMenu PART_ContextMenu;
        private ContentControl PART_DisplayName;
        private ToolBar PART_MainToolbar;
        private MenuItem PART_Mnu_About;
        private MenuItem PART_Mnu_AlwaysOnTop;
        private MenuItem PART_Mnu_AuthList;
        private MenuItem PART_Mnu_AutoCheckUpdates;
        private MenuItem PART_Mnu_AutoClipboard;
        private MenuItem PART_Mnu_AutoSync;
        private MenuItem PART_Mnu_CheckUpdates;
        private MenuItem PART_Mnu_Exit;
        private MenuItem PART_Mnu_Export;
        private MenuItem PART_Mnu_GlobalHotkey;
        private MenuItem PART_Mnu_Import;
        private MenuItem PART_Mnu_New;
        private MenuItem PART_Mnu_Properties;
        private MenuItem PART_Mnu_Restore;
        private MenuItem PART_Mnu_Sync;
        private MenuItem PART_Mnu_Themes;
        private ContentControl PART_Name;
        private ProgressBar PART_Progress;
        private ContentControl PART_Serial;
        private ContentControl PART_Skew;

        private string _theme = "BattleNet";

        public string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                string theme = value;

                if (theme == null)
                    theme = "";

                if (!AvailableThemes.ContainsKey(theme))
                    theme = "BattleNet";

                _theme = theme;

                if (this.IsInitialized)
                    this.LoadTheme();
            }
        }

        private DispatcherTimer timer;

        public AuthenticatorPanel()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);

            this.Theme = Settings.SettingsDatabase.Theme;
        }

        protected void CheckForUpdates(bool silent)
        {
            if (WinBMA.Utilities.UpdateChecker.IsUpdateAvailable)
            {
                UpdateAvailableWindow winUpdate = new UpdateAvailableWindow();
                winUpdate.Owner = App.MainAppWindow;
                winUpdate.Topmost = App.MainAppWindow.Topmost;

                winUpdate.ShowDialog();
            }
            else if (!silent)
            {
                MessageBox.Show("You are using the latest version of WinBMA.", "No Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected void CopyToClipboard()
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator != null && PART_Btn_Code != null && Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                Clipboard.SetText(PART_Btn_Code.Content.ToString());
            }
        }

        private void DecryptAuthenticator()
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator.EncryptionType.HasFlag(AuthAPI.Security.EncryptionProvider.EncryptionType.Password))
            {
                bool firstTry = true;

                while (true)
                {
                    AskPasswordWindow askPassWindow = new AskPasswordWindow(!firstTry);
                    askPassWindow.Title = Settings.SettingsDatabase.SelectedAuthenticator.DisplayName;
                    askPassWindow.Owner = App.MainAppWindow;
                    askPassWindow.Topmost = App.MainAppWindow.Topmost;

                    askPassWindow.ShowDialog();

                    firstTry = false;

                    if (askPassWindow.DialogResult == true)
                    {
                        bool decrypt = Settings.SettingsDatabase.SelectedAuthenticator.Decrypt(askPassWindow.PASS_Decrypt.Password);

                        if (decrypt == true)
                        {
                            this.RefreshAuthenticatorData();
                            break;
                        }
                    }
                    else
                        break;
                }
            }
            else
            {
                bool decrypt = Settings.SettingsDatabase.SelectedAuthenticator.Decrypt();

                if (decrypt == false)
                {
                    string type = Settings.SettingsDatabase.SelectedAuthenticator.EncryptionType.HasFlag(AuthAPI.Security.EncryptionProvider.EncryptionType.LocalUser) ? "user" : "machine";

                    MessageBox.Show("Could not decrypt " + type + " locked authenticator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    this.RefreshAuthenticatorData();
                }
            }
        }

        private void HOTKEY_system_HotKeyPressed(object sender, SystemHotKeyEventArgs e)
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator == null || !Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
                return;

            WindowsInput.InputSimulator.SimulateTextEntry(PART_Btn_Code.Content.ToString());
        }

        private void LoadAuthenticator(int index)
        {
            Settings.SettingsDatabase.SelectedAuthenticatorIndex = index;
            this.RefreshAuthenticatorList();

            if (Settings.SettingsDatabase.SelectedAuthenticator != null && !Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                DecryptAuthenticator();
            }
        }

        private void LoadTheme()
        {
            this.Resources.MergedDictionaries.Clear();

            Uri themeSource = new Uri(String.Format(@"pack://application:,,,/WinBMA;component/Resources/Themes/{0}/{0}.xaml", this.Theme));

            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = themeSource;
            this.Resources.MergedDictionaries.Add(rd);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Retrieve Buttons

            PART_Btn_ContextMenuTarget = this.Template.FindName("PART_Btn_ContextMenuTarget", this) as Button;
            PART_Btn_Code = this.Template.FindName("PART_Btn_Code", this) as Button;

            PART_ContextMenu = this.Template.FindName("PART_ContextMenu", this) as ContextMenu;

            if (PART_Btn_ContextMenuTarget != null && PART_ContextMenu != null)
            {
                PART_Btn_ContextMenuTarget.Click += new RoutedEventHandler(PART_Btn_ContextMenuTarget_Click);
            }

            if (PART_Btn_Code != null)
            {
                PART_Btn_Code.Click += new RoutedEventHandler(PART_Btn_Code_Click);
            }

            // Retrieve Menus

            PART_Mnu_AuthList = this.Template.FindName("PART_Mnu_AuthList", this) as MenuItem;
            PART_Mnu_New = this.Template.FindName("PART_Mnu_New", this) as MenuItem;
            PART_Mnu_Restore = this.Template.FindName("PART_Mnu_Restore", this) as MenuItem;
            PART_Mnu_Import = this.Template.FindName("PART_Mnu_Import", this) as MenuItem;
            PART_Mnu_Sync = this.Template.FindName("PART_Mnu_Sync", this) as MenuItem;
            PART_Mnu_Export = this.Template.FindName("PART_Mnu_Export", this) as MenuItem;
            PART_Mnu_Properties = this.Template.FindName("PART_Mnu_Properties", this) as MenuItem;
            PART_Mnu_AlwaysOnTop = this.Template.FindName("PART_Mnu_AlwaysOnTop", this) as MenuItem;
            PART_Mnu_AutoClipboard = this.Template.FindName("PART_Mnu_AutoClipboard", this) as MenuItem;
            PART_Mnu_GlobalHotkey = this.Template.FindName("PART_Mnu_GlobalHotkey", this) as MenuItem;
            PART_Mnu_AutoSync = this.Template.FindName("PART_Mnu_AutoSync", this) as MenuItem;
            PART_Mnu_AutoCheckUpdates = this.Template.FindName("PART_Mnu_AutoCheckUpdates", this) as MenuItem;
            PART_Mnu_Themes = this.Template.FindName("PART_Mnu_Themes", this) as MenuItem;
            PART_Mnu_CheckUpdates = this.Template.FindName("PART_Mnu_CheckUpdates", this) as MenuItem;
            PART_Mnu_About = this.Template.FindName("PART_Mnu_About", this) as MenuItem;
            PART_Mnu_Exit = this.Template.FindName("PART_Mnu_Exit", this) as MenuItem;

            if (PART_Mnu_New != null)
            {
                PART_Mnu_New.Click += new RoutedEventHandler(PART_Mnu_New_Click);
            }

            if (PART_Mnu_Import != null)
            {
                PART_Mnu_Import.Click += new RoutedEventHandler(PART_Mnu_Import_Click);
            }

            if (PART_Mnu_Restore != null)
            {
                PART_Mnu_Restore.Click += new RoutedEventHandler(PART_Mnu_Restore_Click);
            }

            if (PART_Mnu_Export != null)
            {
                PART_Mnu_Export.Click += new RoutedEventHandler(PART_Mnu_Export_Click);
            }

            if (PART_Mnu_Sync != null)
            {
                PART_Mnu_Sync.Click += new RoutedEventHandler(PART_Mnu_Sync_Click);
            }

            if (PART_Mnu_Properties != null)
            {
                PART_Mnu_Properties.Click += new RoutedEventHandler(PART_Mnu_Properties_Click);
            }

            if (PART_Mnu_AlwaysOnTop != null)
            {
                PART_Mnu_AlwaysOnTop.IsChecked = Settings.SettingsDatabase.AlwaysOnTop;
                PART_Mnu_AlwaysOnTop.Checked += new RoutedEventHandler(PART_Mnu_AlwaysOnTop_Changed);
                PART_Mnu_AlwaysOnTop.Unchecked += new RoutedEventHandler(PART_Mnu_AlwaysOnTop_Changed);
            }

            if (PART_Mnu_AutoClipboard != null)
            {
                PART_Mnu_AutoClipboard.IsChecked = Settings.SettingsDatabase.AutoCopyToClipboard;
                PART_Mnu_AutoClipboard.Checked += new RoutedEventHandler(PART_Mnu_AutoClipboard_Changed);
                PART_Mnu_AutoClipboard.Unchecked += new RoutedEventHandler(PART_Mnu_AutoClipboard_Changed);
            }

            if (PART_Mnu_GlobalHotkey != null)
            {
                PART_Mnu_GlobalHotkey.IsChecked = Settings.SettingsDatabase.IsHotkeyEnabled;
                PART_Mnu_GlobalHotkey.Header = "Enable Global Hotkey (" + Settings.SettingsDatabase.HotkeyString + ")";
                PART_Mnu_GlobalHotkey.Click += new RoutedEventHandler(PART_Mnu_GlobalHotkey_Click);
            }

            if (PART_Mnu_AutoSync != null)
            {
                PART_Mnu_AutoSync.IsChecked = Settings.SettingsDatabase.AutoSyncTime;
                PART_Mnu_AutoSync.Checked += new RoutedEventHandler(PART_Mnu_AutoSync_Changed);
                PART_Mnu_AutoSync.Unchecked += new RoutedEventHandler(PART_Mnu_AutoSync_Changed);
            }

            if (PART_Mnu_AutoCheckUpdates != null)
            {
                PART_Mnu_AutoCheckUpdates.IsChecked = Settings.SettingsDatabase.CheckForUpdates;
                PART_Mnu_AutoCheckUpdates.Checked += new RoutedEventHandler(PART_Mnu_AutoCheckUpdates_Changed);
                PART_Mnu_AutoCheckUpdates.Unchecked += new RoutedEventHandler(PART_Mnu_AutoCheckUpdates_Changed);
            }

            if (PART_Mnu_Themes != null)
            {
                PART_Mnu_Themes.Items.Clear();

                foreach (string theme in AvailableThemes.Keys)
                {
                    MenuItem PART_Mnu_Themes_Item = new MenuItem();
                    PART_Mnu_Themes_Item.Header = AvailableThemes[theme];
                    PART_Mnu_Themes_Item.Tag = theme;

                    if (this.Theme == theme)
                        PART_Mnu_Themes_Item.IsChecked = true;

                    PART_Mnu_Themes_Item.Click += new RoutedEventHandler(PART_Mnu_Themes_Item_Click);

                    PART_Mnu_Themes.Items.Add(PART_Mnu_Themes_Item);
                }
            }

            if (PART_Mnu_CheckUpdates != null)
            {
                PART_Mnu_CheckUpdates.Click += new RoutedEventHandler(PART_Mnu_CheckUpdates_Click);
            }

            if (PART_Mnu_About != null)
            {
                PART_Mnu_About.Click += new RoutedEventHandler(PART_Mnu_About_Click);
            }

            if (PART_Mnu_Exit != null)
            {
                PART_Mnu_Exit.Click += new RoutedEventHandler(PART_Mnu_Exit_Click);
            }

            // Retrieve Combo Box

            PART_Cmb_AuthList = this.Template.FindName("PART_Cmb_AuthList", this) as ComboBox;

            if (PART_Cmb_AuthList != null)
            {
                PART_Cmb_AuthList.SelectionChanged += new SelectionChangedEventHandler(PART_Cmb_AuthList_SelectionChanged);
            }

            // Retrieve Progress Bar

            PART_Progress = this.Template.FindName("PART_Progress", this) as ProgressBar;

            if (PART_Progress != null)
            {
                PART_Progress.Minimum = 0;
                PART_Progress.Maximum = WinBMA.AuthAPI.BlizzardAPI.HOTP_PERIOD_LENGTH - 1;
            }

            // Retrieve Labels

            PART_DisplayName = this.Template.FindName("PART_DisplayName", this) as ContentControl;
            PART_CanonicalName = this.Template.FindName("PART_CanonicalName", this) as ContentControl;
            PART_Name = this.Template.FindName("PART_Name", this) as ContentControl;
            PART_Serial = this.Template.FindName("PART_Serial", this) as ContentControl;
            PART_Skew = this.Template.FindName("PART_Skew", this) as ContentControl;

            // Retrieve Toolbar

            PART_MainToolbar = this.Template.FindName("PART_MainToolbar", this) as ToolBar;

            if (PART_MainToolbar != null)
            {
                PART_MainToolbar.Loaded += new RoutedEventHandler(PART_MainToolbar_Loaded);
            }

            this.RefreshAuthenticatorList();
        }

        public void OnParentContentRendered()
        {
            this.LoadAuthenticator(Settings.SettingsDatabase.SelectedAuthenticatorIndex);

            if (Settings.SettingsDatabase.IsUpdateCheckNeeded)
            {
                CheckForUpdates(true);
            }

            HOTKEY_system = new SystemHotKey(App.MainAppWindow);
            HOTKEY_system.HotKeyPressed += new EventHandler<SystemHotKeyEventArgs>(HOTKEY_system_HotKeyPressed);

            HOTKEY_system.Modifiers = Settings.SettingsDatabase.HotkeyModifiers;
            HOTKEY_system.Key = Settings.SettingsDatabase.Hotkey;
            HOTKEY_system.Enabled = Settings.SettingsDatabase.IsHotkeyEnabled;
        }

        private void PART_Btn_Code_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator != null)
            {
                if (Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
                    CopyToClipboard();
                else
                    DecryptAuthenticator();
            }
        }

        private void PART_Btn_ContextMenuTarget_Click(object sender, RoutedEventArgs e)
        {
            PART_ContextMenu.PlacementTarget = PART_Btn_ContextMenuTarget;
            PART_ContextMenu.IsOpen = true;
        }

        private void PART_Cmb_AuthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ignoreIndexChange)
                return;

            LoadAuthenticator(PART_Cmb_AuthList.SelectedIndex);
        }

        private void PART_MainToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;

            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void PART_Mnu_About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow winAbout = new AboutWindow();
            winAbout.Owner = App.MainAppWindow;
            winAbout.Topmost = App.MainAppWindow.Topmost;

            winAbout.ShowDialog();
        }

        private void PART_Mnu_AlwaysOnTop_Changed(object sender, RoutedEventArgs e)
        {
            Settings.SettingsDatabase.AlwaysOnTop = PART_Mnu_AlwaysOnTop.IsChecked;
            App.MainAppWindow.Topmost = Settings.SettingsDatabase.AlwaysOnTop;
        }

        private void PART_Mnu_AuthList_Item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem PART_Mnu_AuthList_Item = e.Source as MenuItem;
            LoadAuthenticator(Convert.ToInt32(PART_Mnu_AuthList_Item.Tag));
        }

        private void PART_Mnu_AutoCheckUpdates_Changed(object sender, RoutedEventArgs e)
        {
            Settings.SettingsDatabase.CheckForUpdates = PART_Mnu_AutoCheckUpdates.IsChecked;

            if (Settings.SettingsDatabase.IsUpdateCheckNeeded)
            {
                CheckForUpdates(true);
            }
        }

        private void PART_Mnu_AutoClipboard_Changed(object sender, RoutedEventArgs e)
        {
            Settings.SettingsDatabase.AutoCopyToClipboard = PART_Mnu_AutoClipboard.IsChecked;

            if (Settings.SettingsDatabase.AutoCopyToClipboard == true)
                CopyToClipboard();
        }

        private void PART_Mnu_AutoClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.SettingsDatabase.AutoCopyToClipboard = false;
        }

        private void PART_Mnu_AutoSync_Changed(object sender, RoutedEventArgs e)
        {
            Settings.SettingsDatabase.AutoSyncTime = PART_Mnu_AutoCheckUpdates.IsChecked;
        }

        private void PART_Mnu_CheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdates(false);
        }

        private void PART_Mnu_Exit_Click(object sender, RoutedEventArgs e)
        {
            App.MainAppWindow.Close();
        }

        private void PART_Mnu_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportAuthWindow winExport = new ExportAuthWindow();
            winExport.Owner = App.MainAppWindow;
            winExport.Topmost = App.MainAppWindow.Topmost;

            winExport.ShowDialog();
        }

        private void PART_Mnu_GlobalHotkey_Click(object sender, RoutedEventArgs e)
        {
            EditHotKeyWindow winEditHotKey = new EditHotKeyWindow();
            winEditHotKey.Owner = App.MainAppWindow;
            winEditHotKey.Topmost = App.MainAppWindow.Topmost;

            winEditHotKey.ShowDialog();

            if (winEditHotKey.DialogResult == true)
            {
                HOTKEY_system.Enabled = false;

                Settings.SettingsDatabase.IsHotkeyEnabled = (bool)winEditHotKey.CHECK_EnableHotkey.IsChecked;

                if (Settings.SettingsDatabase.IsHotkeyEnabled)
                {
                    Settings.SettingsDatabase.HotkeyModifiers = winEditHotKey.ModifierKeys;
                    Settings.SettingsDatabase.Hotkey = winEditHotKey.Keys;

                    HOTKEY_system.Modifiers = Settings.SettingsDatabase.HotkeyModifiers;
                    HOTKEY_system.Key = Settings.SettingsDatabase.Hotkey;
                    HOTKEY_system.Enabled = Settings.SettingsDatabase.IsHotkeyEnabled;
                }

                PART_Mnu_GlobalHotkey.IsChecked = Settings.SettingsDatabase.IsHotkeyEnabled;
                PART_Mnu_GlobalHotkey.Header = "Enable Global Hotkey (" + Settings.SettingsDatabase.HotkeyString + ")";
            }
        }

        private void PART_Mnu_Import_Click(object sender, RoutedEventArgs e)
        {
            ImportAuthWindow winImportAuth = new ImportAuthWindow();
            winImportAuth.Owner = App.MainAppWindow;
            winImportAuth.Topmost = App.MainAppWindow.Topmost;

            winImportAuth.ShowDialog();

            if (winImportAuth.DialogResult == true)
            {
                Settings.SettingsDatabase.SelectedAuthenticatorIndex = Settings.SettingsDatabase.Authenticators.Count - 1;
                this.LoadAuthenticator(Settings.SettingsDatabase.SelectedAuthenticatorIndex);
            }
        }

        private void PART_Mnu_New_Click(object sender, RoutedEventArgs e)
        {
            NewAuthWindow winNewAuth = new NewAuthWindow();
            winNewAuth.Owner = App.MainAppWindow;
            winNewAuth.Topmost = App.MainAppWindow.Topmost;

            winNewAuth.ShowDialog();

            if (winNewAuth.DialogResult == true)
            {
                Settings.SettingsDatabase.SelectedAuthenticatorIndex = Settings.SettingsDatabase.Authenticators.Count - 1;
                this.LoadAuthenticator(Settings.SettingsDatabase.SelectedAuthenticatorIndex);
            }
        }

        private void PART_Mnu_Properties_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow winProperty = new PropertyWindow();
            winProperty.Owner = App.MainAppWindow;
            winProperty.Topmost = App.MainAppWindow.Topmost;

            winProperty.ShowDialog();

            if (winProperty.DialogResult == true)
            {
                this.LoadAuthenticator(Settings.SettingsDatabase.SelectedAuthenticatorIndex);
            }
        }

        private void PART_Mnu_Restore_Click(object sender, RoutedEventArgs e)
        {
            RestoreAuthWindow winRestoreAuth = new RestoreAuthWindow();
            winRestoreAuth.Owner = App.MainAppWindow;
            winRestoreAuth.Topmost = App.MainAppWindow.Topmost;

            winRestoreAuth.ShowDialog();

            if (winRestoreAuth.DialogResult == true)
            {
                Settings.SettingsDatabase.SelectedAuthenticatorIndex = Settings.SettingsDatabase.Authenticators.Count - 1;
                this.LoadAuthenticator(Settings.SettingsDatabase.SelectedAuthenticatorIndex);
            }
        }

        private void PART_Mnu_Sync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthAPI.BlizzardAPI.SyncClock();

                MessageBox.Show("Clocks successfully synchronized.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occured while trying to sync clocks.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PART_Mnu_Themes_Item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem PART_Mnu_Themes_Item = e.Source as MenuItem;

            Settings.SettingsDatabase.Theme = PART_Mnu_Themes_Item.Tag.ToString();
            this.Theme = Settings.SettingsDatabase.Theme;
        }

        private void RefreshAuthenticatorData()
        {
            if (Settings.SettingsDatabase.SelectedAuthenticator == null || !Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
            {
                if (timer.IsEnabled)
                    timer.Stop();
            }
            else
            {
                if (!timer.IsEnabled)
                    timer.Start();
            }

            if (PART_Btn_Code != null)
            {
                if (Settings.SettingsDatabase.SelectedAuthenticator == null)
                {
                    if (this.Theme == "ClassicWinBMA")
                    {
                        PART_Btn_Code.Content = "No Auth";
                    }
                    else
                    {
                        PART_Btn_Code.Content = "No Authenticator";
                    }
                }
                else
                {
                    if (Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted)
                    {
                        PART_Btn_Code.Content = Settings.SettingsDatabase.SelectedAuthenticator.Key;

                        if (Settings.SettingsDatabase.AutoCopyToClipboard)
                        {
                            CopyToClipboard();
                        }
                    }
                    else
                    {
                        PART_Btn_Code.Content = "Encrypted";
                    }
                }
            }

            if (PART_DisplayName != null)
            {
                PART_DisplayName.Content = (Settings.SettingsDatabase.SelectedAuthenticator == null) ? "" : Settings.SettingsDatabase.SelectedAuthenticator.DisplayName;
            }

            if (PART_CanonicalName != null)
            {
                PART_CanonicalName.Content = (Settings.SettingsDatabase.SelectedAuthenticator == null) ? "" : Settings.SettingsDatabase.SelectedAuthenticator.CanonicalName;
            }

            if (PART_Name != null)
            {
                PART_Name.Content = (Settings.SettingsDatabase.SelectedAuthenticator == null) ? "" : Settings.SettingsDatabase.SelectedAuthenticator.Name;
            }

            if (PART_Serial != null)
            {
                PART_Serial.Content = (Settings.SettingsDatabase.SelectedAuthenticator == null) ? "" : Settings.SettingsDatabase.SelectedAuthenticator.Serial;
            }

            if (PART_Skew != null)
            {
                PART_Skew.Content = (Settings.SettingsDatabase.ServerTimeOffset / 1000F).ToString("0.000") + "s";
            }

            if (PART_Progress != null)
            {
                PART_Progress.Value = (Settings.SettingsDatabase.SelectedAuthenticator == null || !Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted) ? 0 : WinBMA.AuthAPI.BlizzardAPI.MillisecondsSincePeriod;
            }

            if (PART_Mnu_Export != null)
            {
                if (Settings.SettingsDatabase.SelectedAuthenticator == null)
                    PART_Mnu_Export.IsEnabled = false;
                else
                    PART_Mnu_Export.IsEnabled = Settings.SettingsDatabase.SelectedAuthenticator.IsDecrypted;
            }

            if (PART_Mnu_Properties != null)
            {
                PART_Mnu_Properties.IsEnabled = (Settings.SettingsDatabase.SelectedAuthenticator != null);
            }
        }

        private void RefreshAuthenticatorList()
        {
            if (PART_Mnu_AuthList != null)
            {
                PART_Mnu_AuthList.Items.Clear();

                int index = 0;

                foreach (WinBMA.AuthAPI.Authenticator auth in Settings.SettingsDatabase.Authenticators)
                {
                    MenuItem PART_Mnu_AuthList_Item = new MenuItem();
                    PART_Mnu_AuthList_Item.Header = auth.DisplayName;
                    PART_Mnu_AuthList_Item.Tag = index;

                    if (Settings.SettingsDatabase.SelectedAuthenticatorIndex == index)
                        PART_Mnu_AuthList_Item.IsChecked = true;

                    PART_Mnu_AuthList_Item.Click += new RoutedEventHandler(PART_Mnu_AuthList_Item_Click);

                    PART_Mnu_AuthList.Items.Add(PART_Mnu_AuthList_Item);

                    index++;
                }

                PART_Mnu_AuthList.IsEnabled = PART_Mnu_AuthList.HasItems;
            }

            if (PART_Cmb_AuthList != null)
            {
                _ignoreIndexChange = true;

                PART_Cmb_AuthList.Items.Clear();

                int index = 0;

                foreach (WinBMA.AuthAPI.Authenticator auth in Settings.SettingsDatabase.Authenticators)
                {
                    ComboBoxItem PART_Cmb_AuthList_Item = new ComboBoxItem();
                    PART_Cmb_AuthList_Item.Content = auth.DisplayName;

                    if (Settings.SettingsDatabase.SelectedAuthenticatorIndex == index)
                        PART_Cmb_AuthList_Item.IsSelected = true;

                    PART_Cmb_AuthList.Items.Add(PART_Cmb_AuthList_Item);

                    index++;
                }

                PART_Cmb_AuthList.IsEnabled = PART_Cmb_AuthList.HasItems;

                _ignoreIndexChange = false;
            }

            this.RefreshAuthenticatorData();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            long oldValue = 0;
            long currentValue = WinBMA.AuthAPI.BlizzardAPI.MillisecondsSincePeriod;

            if (PART_Progress != null)
            {
                oldValue = (long)PART_Progress.Value;
                PART_Progress.Value = currentValue;
            }

            if (oldValue > currentValue)
            {
                this.RefreshAuthenticatorData();
            }
        }
    }
}