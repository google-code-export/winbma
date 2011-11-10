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
using WinBMA.Utilities;

namespace WinBMA.UI
{
    public partial class EditHotKeyWindow : Window
    {
        public Utilities.Keys Keys
        {
            get { return (Utilities.Keys)Enum.Parse(typeof(Utilities.Keys), COMBO_Keys.SelectedItem.ToString()); }
        }

        public Utilities.SystemHotKey.ModifierKeys ModifierKeys
        {
            get
            {
                Utilities.SystemHotKey.ModifierKeys modKeys = SystemHotKey.ModifierKeys.None;

                if (CHECK_Alt.IsChecked == true)
                    modKeys |= Utilities.SystemHotKey.ModifierKeys.Alt;

                if (CHECK_Ctrl.IsChecked == true)
                    modKeys |= Utilities.SystemHotKey.ModifierKeys.Control;

                if (CHECK_Shift.IsChecked == true)
                    modKeys |= Utilities.SystemHotKey.ModifierKeys.Shift;

                if (CHECK_Win.IsChecked == true)
                    modKeys |= Utilities.SystemHotKey.ModifierKeys.Windows;

                return modKeys;
            }
        }

        public EditHotKeyWindow()
        {
            InitializeComponent();

            string[] keys = Enum.GetNames(typeof(Keys));
            Array.Sort(keys);

            foreach (string key in keys)
            {
                COMBO_Keys.Items.Add(key);
            }

            CHECK_EnableHotkey.IsChecked = Settings.SettingsDatabase.IsHotkeyEnabled;

            CHECK_Alt.IsChecked = Settings.SettingsDatabase.HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Alt);
            CHECK_Ctrl.IsChecked = Settings.SettingsDatabase.HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Control);
            CHECK_Shift.IsChecked = Settings.SettingsDatabase.HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Shift);
            CHECK_Win.IsChecked = Settings.SettingsDatabase.HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Windows);

            COMBO_Keys.SelectedItem = Settings.SettingsDatabase.Hotkey.ToString();

            CHECK_EnableHotkey.Checked += new RoutedEventHandler(CHECK_Changed);
            CHECK_EnableHotkey.Unchecked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Alt.Checked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Alt.Unchecked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Ctrl.Checked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Ctrl.Unchecked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Shift.Checked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Shift.Unchecked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Win.Checked += new RoutedEventHandler(CHECK_Changed);
            CHECK_Win.Unchecked += new RoutedEventHandler(CHECK_Changed);

            COMBO_Keys.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(COMBO_Keys_SelectionChanged);

            BUTTON_Ok.Click += new RoutedEventHandler(BUTTON_Ok_Click);

            Validate();
        }

        private void BUTTON_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CHECK_Changed(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        private void COMBO_Keys_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Validate();
        }

        private void Validate()
        {
            if (CHECK_EnableHotkey.IsChecked == false)
            {
                BUTTON_Ok.IsEnabled = true;
            }
            else
            {
                try
                {
                    BUTTON_Ok.IsEnabled = ((bool)CHECK_Alt.IsChecked || (bool)CHECK_Ctrl.IsChecked || (bool)CHECK_Shift.IsChecked ||
                                       (bool)CHECK_Win.IsChecked) && COMBO_Keys.SelectedItem != null;
                }
                catch (InvalidOperationException)
                {
                    BUTTON_Ok.IsEnabled = false;
                }
            }
        }
    }
}