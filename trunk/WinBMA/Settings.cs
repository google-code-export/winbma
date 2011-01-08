using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinBMA
{
    public static class Settings
    {
        private static List<FriendlyAuth> _auths;
        private static int _lastSelectedIndex;
        private static DateTime[] _lastTimeSyncTime;
        private static long[] _timeDiffs;


        public static void Load()
        {
            string settingsFile = SettingsPath + "\\settings.cfg";

            LoadDefaults();

            if (!File.Exists(SettingsPath + "\\settings.cfg")) return;

            using (BinaryReader binReader = new BinaryReader(File.OpenRead(settingsFile)))
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

                int numOfAuths = binReader.ReadInt32();

                for (int i = 0; i < numOfAuths; i++)
                {
                    string name = binReader.ReadString();
                    string serial = binReader.ReadString();
                    string token = BlizzAuth.Helper.ConvertBytesToHexString(binReader.ReadBytes(20));

                    _auths.Add(new FriendlyAuth(new BlizzAuth.Authenticator(serial, token), name));
                }

                for (int i = 0; i < 2; i++)
                {
                    _lastTimeSyncTime[i] = DateTime.FromBinary(binReader.ReadInt64());
                    _timeDiffs[i] = binReader.ReadInt64();
                }

                if (fileVersion > 1)
                {
                    _lastSelectedIndex = binReader.ReadInt32();
                }
            }
        }

        public static void Save()
        {
            string settingsFile = SettingsPath + "\\settings.cfg";

            using (BinaryWriter binWriter = new BinaryWriter(File.Create(settingsFile)))
            {
                binWriter.Write("WINBMACFG".ToCharArray());
                binWriter.Write(2);
                binWriter.Write(_auths.Count);

                foreach (FriendlyAuth auth in _auths)
                {
                    binWriter.Write(auth.FriendlyName);
                    binWriter.Write(auth.Authenticator.Serial);
                    binWriter.Write(BlizzAuth.Helper.ConvertHexStringToBytes(auth.Authenticator.Token));
                }

                for (int i = 0; i < 2; i++)
                {
                    binWriter.Write(_lastTimeSyncTime[i].ToBinary());
                    binWriter.Write(_timeDiffs[i]);
                }

                binWriter.Write(_lastSelectedIndex);
            }
        }

        public static void LoadDefaults()
        {
            _auths = new List<FriendlyAuth>();

            _lastSelectedIndex = -1;
            
            _lastTimeSyncTime = new DateTime[2] { DateTime.MinValue, DateTime.MinValue };
            _timeDiffs = new long[2] { long.MinValue, long.MinValue };
        }

        public static List<FriendlyAuth> Authenticators
        {
            get
            {
                return _auths;
            }
            set
            {
                _auths = value;
            }
        }

        public static DateTime[] TimeSinceLastSync
        {
            get
            {
                return _lastTimeSyncTime;
            }
            set
            {
                _lastTimeSyncTime = value;
            }
        }

        public static long[] TimeDifference
        {
            get
            {
                return _timeDiffs;
            }
            set
            {
                _timeDiffs = value;
            }
        }

        public static int LastSelectedIndex
        {
            get
            {
                return _lastSelectedIndex;
            }
            set
            {
                _lastSelectedIndex = value;
            }
        }

        private static string SettingsPath
        {
            get
            {
                string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + "\\WinBMA";

                if (!Directory.Exists(AppFolder))
                {
                    Directory.CreateDirectory(AppFolder);
                }

                return AppFolder;
            }
        }
    }
}
