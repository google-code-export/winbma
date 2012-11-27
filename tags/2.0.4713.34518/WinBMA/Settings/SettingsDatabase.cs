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
using System.IO;
using System.Text;
using WinBMA.Utilities;

namespace WinBMA.Settings
{
    internal static class SettingsDatabase
    {
        private static bool _alwaysOnTop;

        public static bool AlwaysOnTop
        {
            get
            {
                return _alwaysOnTop;
            }
            set
            {
                _alwaysOnTop = value;
            }
        }

        private static bool attemptedAutoSync = false;

        public static List<AuthAPI.Authenticator> Authenticators
        {
            get
            {
                if (_authList == null)
                {
                    _authList = new List<AuthAPI.Authenticator>();
                }

                return _authList;
            }
            set
            {
                _authList = value;
            }
        }

        private static List<AuthAPI.Authenticator> _authList;

        private static bool _autoCopyToClipboard;

        public static bool AutoCopyToClipboard
        {
            get
            {
                return _autoCopyToClipboard;
            }
            set
            {
                _autoCopyToClipboard = value;
            }
        }

        private static bool _autoSync;

        public static bool AutoSyncTime
        {
            get
            {
                return _autoSync;
            }
            set
            {
                _autoSync = value;
            }
        }

        private static bool _checkForUpdates;

        public static bool CheckForUpdates
        {
            get
            {
                return _checkForUpdates;
            }
            set
            {
                _checkForUpdates = value;
            }
        }

        private static Keys _hotkey;

        public static Keys Hotkey
        {
            get
            {
                return _hotkey;
            }
            set
            {
                _hotkey = value;
            }
        }

        private static bool _hotkeyEnabled;

        private static SystemHotKey.ModifierKeys _hotkeyModifiers;

        public static SystemHotKey.ModifierKeys HotkeyModifiers
        {
            get
            {
                return _hotkeyModifiers;
            }
            set
            {
                _hotkeyModifiers = value;
            }
        }

        public static string HotkeyString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Control))
                    builder.Append("CTRL + ");

                if (HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Control))
                    builder.Append("SHIFT + ");

                if (HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Alt))
                    builder.Append("ALT + ");

                if (HotkeyModifiers.HasFlag(SystemHotKey.ModifierKeys.Windows))
                    builder.Append("WIN + ");

                builder.Append(Hotkey.ToString());

                return builder.ToString();
            }
        }

        public static bool IsHotkeyEnabled
        {
            get
            {
                return _hotkeyEnabled;
            }
            set
            {
                _hotkeyEnabled = value;
            }
        }

        public static bool IsSyncNeeded
        {
            get
            {
                if (attemptedAutoSync == true)
                    return false;

                if (LastSyncTime == DateTime.MinValue)
                    return true;

                if (AutoSyncTime == false)
                    return false;

                return (DateTime.Now - LastSyncTime) >= TimeSpan.FromDays(7);
            }
        }

        public static bool IsUpdateCheckNeeded
        {
            get
            {
                if (CheckForUpdates == false)
                    return false;

                return (DateTime.Now - LastUpdateCheck) >= TimeSpan.FromDays(7);
            }
        }

        private static Int32 _lastSelectedIndex;

        private static DateTime _lastSyncTime;

        public static DateTime LastSyncTime
        {
            get
            {
                return _lastSyncTime;
            }
        }

        private static DateTime _lastUpdateCheck;

        public static DateTime LastUpdateCheck
        {
            get
            {
                return _lastUpdateCheck;
            }
            set
            {
                _lastUpdateCheck = value;
            }
        }

        public static AuthAPI.Authenticator SelectedAuthenticator
        {
            get
            {
                if (SelectedAuthenticatorIndex < 0)
                    return null;

                return Authenticators[SelectedAuthenticatorIndex];
            }
            set
            {
                if (value == null)
                    SelectedAuthenticatorIndex = -1;
                else if (Authenticators.Contains(value))
                    SelectedAuthenticatorIndex = Authenticators.IndexOf(value);
                else
                    SelectedAuthenticatorIndex = -1;
            }
        }

        public static Int32 SelectedAuthenticatorIndex
        {
            get
            {
                if (_lastSelectedIndex < 0)
                    return -1;
                else if (_lastSelectedIndex >= Authenticators.Count)
                    return -1;

                return _lastSelectedIndex;
            }
            set
            {
                _lastSelectedIndex = value;

                if (_lastSelectedIndex < 0)
                    _lastSelectedIndex = -1;
                else if (_lastSelectedIndex >= Authenticators.Count)
                    _lastSelectedIndex = -1;
            }
        }

        public static Int64 ServerTimeOffset
        {
            get
            {
                if (IsSyncNeeded)
                {
                    attemptedAutoSync = true;

                    try
                    {
                        AuthAPI.BlizzardAPI.SyncClock();
                    }
                    catch (Exception) { }
                }

                return _timeOffset;
            }
            set
            {
                _timeOffset = value;
                _lastSyncTime = DateTime.Now;
            }
        }

        private static string _settingsFile;

        private static string SettingsFile
        {
            get
            {
                if (_settingsFile == null)
                {
                    string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + "\\WinBMA";

                    if (!Directory.Exists(settingsFolder))
                    {
                        Directory.CreateDirectory(settingsFolder);
                    }

                    _settingsFile = settingsFolder + "\\settings.cfg";
                }

                return _settingsFile;
            }
        }

        private static String _theme;

        public static string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        private static Int64 _timeOffset;

        private static void InitializeDefaults()
        {
            _authList = null;

            _lastSyncTime = DateTime.MinValue;
            _timeOffset = 0;
            _autoSync = true;

            _lastSelectedIndex = -1;

            _alwaysOnTop = false;
            _theme = "BattleNet";
            _autoCopyToClipboard = false;

            _lastUpdateCheck = DateTime.MinValue;
            _checkForUpdates = true;

            _hotkeyModifiers = SystemHotKey.ModifierKeys.Windows;
            _hotkey = Keys.Enter;
            _hotkeyEnabled = true;
        }

        public static void Load()
        {
            InitializeDefaults();

            if (!File.Exists(SettingsFile)) return;

            using (BinaryReader binReader = new BinaryReader(File.OpenRead(SettingsFile)))
            {
                string fileHeader = "";

                for (int i = 0; i < 9; i++)
                {
                    fileHeader += binReader.ReadChar();
                }

                if (fileHeader != "WINBMACFG")
                {
                    return;
                }

                int fileVersion = binReader.ReadInt32();

                if (fileVersion > 4)
                {
                    System.Windows.MessageBox.Show("The settings database was created with a newer version of WinBMA. We were unable to load your settings.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }

                int numOfAuths = binReader.ReadInt32();

                for (int i = 0; i < numOfAuths; i++)
                {
                    string name = binReader.ReadString();
                    string serial = binReader.ReadString();
                    bool? isRestorable = null;
                    AuthAPI.Security.EncryptionProvider.EncryptionType encType = AuthAPI.Security.EncryptionProvider.EncryptionType.None;
                    int tokenLen = 20;

                    if (fileVersion > 2)
                    {
                        isRestorable = binReader.ReadBoolean();
                        encType = (AuthAPI.Security.EncryptionProvider.EncryptionType)binReader.ReadByte();
                        tokenLen = binReader.ReadInt32();
                    }

                    byte[] token = binReader.ReadBytes(tokenLen);

                    Authenticators.Add(new AuthAPI.Authenticator(name, serial, token, isRestorable, encType));
                }

                if (fileVersion < 3)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        DateTime storedLastSync = DateTime.FromBinary(binReader.ReadInt64());
                        Int64 storedOffset = binReader.ReadInt64();

                        if (storedLastSync > _lastSyncTime)
                        {
                            _lastSyncTime = storedLastSync;
                            _timeOffset = storedOffset;
                        }
                    }
                }
                else
                {
                    _lastSyncTime = DateTime.FromBinary(binReader.ReadInt64());
                    _timeOffset = binReader.ReadInt64();
                }

                if (fileVersion == 1)
                    return;

                SelectedAuthenticatorIndex = binReader.ReadInt32();

                if (fileVersion == 2)
                    return;

                _autoSync = binReader.ReadBoolean();

                _alwaysOnTop = binReader.ReadBoolean();
                _autoCopyToClipboard = binReader.ReadBoolean();
                _theme = binReader.ReadString();

                _checkForUpdates = binReader.ReadBoolean();
                _lastUpdateCheck = DateTime.FromBinary(binReader.ReadInt64());

                if (fileVersion == 3)
                    return;

                _hotkeyEnabled = binReader.ReadBoolean();
                _hotkeyModifiers = (Utilities.SystemHotKey.ModifierKeys)binReader.ReadByte();
                _hotkey = (Utilities.Keys)binReader.ReadInt32();
            }
        }

        public static void Save()
        {
            using (BinaryWriter binWriter = new BinaryWriter(File.Create(SettingsFile)))
            {
                binWriter.Write("WINBMACFG".ToCharArray());
                binWriter.Write(4);
                binWriter.Write(Authenticators.Count);

                foreach (AuthAPI.Authenticator auth in Authenticators)
                {
                    if (auth.Name != null)
                        binWriter.Write(auth.Name);
                    else
                        binWriter.Write("");

                    binWriter.Write(auth.Serial);
                    binWriter.Write(auth.IsRestoreSupported);
                    binWriter.Write((byte)auth.EncryptionType);
                    binWriter.Write(auth.Token.Length);
                    binWriter.Write(auth.Token);
                }

                binWriter.Write(LastSyncTime.ToBinary());
                binWriter.Write(ServerTimeOffset);
                binWriter.Write(SelectedAuthenticatorIndex);
                binWriter.Write(AutoSyncTime);
                binWriter.Write(AlwaysOnTop);
                binWriter.Write(AutoCopyToClipboard);
                binWriter.Write(Theme);
                binWriter.Write(CheckForUpdates);
                binWriter.Write(LastUpdateCheck.ToBinary());

                binWriter.Write(IsHotkeyEnabled);
                binWriter.Write((byte)HotkeyModifiers);
                binWriter.Write((int)Hotkey);
            }
        }
    }
}