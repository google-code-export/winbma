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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WinBMA.AuthAPI
{
    public class Authenticator : ICloneable
    {
        public string CanonicalName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.Name))
                    return this.Serial;

                return String.Format("{0} ({1})", this.Name, this.Serial);
            }
        }

        private byte[] _decryptedToken;

        public byte[] DecryptedToken
        {
            get
            {
                return _decryptedToken;
            }
        }

        public string DisplayName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.Name))
                    return this.Serial;

                return this.Name;
            }
        }

        private Security.EncryptionProvider.EncryptionType _encryptionType;

        public Security.EncryptionProvider.EncryptionType EncryptionType
        {
            get
            {
                return _encryptionType;
            }
            private set
            {
                _encryptionType = value;

                if (_encryptionType == Security.EncryptionProvider.EncryptionType.None)
                    _decryptedToken = (byte[])_token.Clone();
            }
        }

        public bool IsDecrypted
        {
            get
            {
                return !(_decryptedToken == null);
            }
        }

        private bool _isRestoreSupported;

        public bool IsRestoreSupported
        {
            get
            {
                return _isRestoreSupported;
            }
            private set
            {
                _isRestoreSupported = value;
            }
        }

        public string Key
        {
            get
            {
                if (!this.IsDecrypted)
                    return null;

                return this.GetKey(0);
            }
        }

        private string _name;

        public string Name
        {
            get
            {
                if (_name == Serial)
                    return null;

                return _name;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    _name = null;
                else
                    _name = value;
            }
        }

        private AuthenticatorRegion _region;

        public AuthenticatorRegion Region
        {
            get
            {
                if (_region == null)
                {
                    string regionIdentifier = this.Serial.Substring(0, 2);

                    _region = AuthenticatorRegion.Factory(regionIdentifier);
                }

                return _region;
            }
        }

        private string _restoreCode;

        public string RestoreCode
        {
            get
            {
                if (!IsDecrypted)
                    return null;

                if (_restoreCode == null)
                {
                    byte[] serial = Encoding.UTF8.GetBytes(this.Serial.ToUpperInvariant().Replace("-", ""));

                    byte[] signature = new byte[serial.Length + DecryptedToken.Length];
                    Array.Copy(serial, signature, serial.Length);
                    Array.Copy(DecryptedToken, 0, signature, serial.Length, DecryptedToken.Length);

                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    byte[] digestResult = sha1.ComputeHash(signature);

                    _restoreCode = Security.RestoreCodeProvider.FromBytes(digestResult);
                }

                return _restoreCode;
            }
        }

        private string _serial;

        public string Serial
        {
            get
            {
                return _serial;
            }
            private set
            {
                _region = null;
                _restoreCode = null;
                _serial = value;
            }
        }

        private byte[] _token;

        public byte[] Token
        {
            get
            {
                return _token;
            }
            private set
            {
                _token = value;
                _decryptedToken = null;
                _restoreCode = null;
            }
        }

        public Authenticator(string serial, byte[] token)
        {
            Initialize(null, serial, token, true, Security.EncryptionProvider.EncryptionType.None);
        }

        public Authenticator(string name, string serial, byte[] token)
        {
            Initialize(name, serial, token, null, Security.EncryptionProvider.EncryptionType.None);
        }

        public Authenticator(string name, string serial, byte[] token, bool? supportsRestore)
        {
            Initialize(name, serial, token, supportsRestore, Security.EncryptionProvider.EncryptionType.None);
        }

        public Authenticator(string name, string serial, byte[] token, bool? supportsRestore, Security.EncryptionProvider.EncryptionType encryptionType)
        {
            Initialize(name, serial, token, supportsRestore, encryptionType);
        }

        public static Authenticator FromFile(string filePath)
        {
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.OpenRead(filePath)))
                {
                    string fileHeader = "";

                    for (int i = 0; i < 9; i++)
                    {
                        fileHeader += binReader.ReadChar();
                    }

                    if (fileHeader != "WINBMAEXP")
                    {
                        return null;
                    }

                    int fileVersion = binReader.ReadInt32();

                    if (fileVersion > 2)
                        return null;

                    string name = binReader.ReadString();
                    string serial = binReader.ReadString();

                    bool? isRestorable = null;
                    AuthAPI.Security.EncryptionProvider.EncryptionType encType = AuthAPI.Security.EncryptionProvider.EncryptionType.None;
                    int tokenLen = 20;

                    if (fileVersion > 1)
                    {
                        isRestorable = binReader.ReadBoolean();
                        encType = (AuthAPI.Security.EncryptionProvider.EncryptionType)binReader.ReadByte();
                        tokenLen = binReader.ReadInt32();
                    }

                    byte[] token = binReader.ReadBytes(tokenLen);

                    return new Authenticator(name, serial, token, isRestorable, encType);
                }
            }
            catch (Exception) { return null; }
        }

        public static bool IsValidSerial(string serial)
        {
            if (serial.Length != 17) return false;

            Regex rxTest = new Regex("^(EU|US)-[0-9]{4}-[0-9]{4}-[0-9]{4}$", RegexOptions.IgnoreCase);
            return rxTest.IsMatch(serial);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool Decrypt()
        {
            return Decrypt(null);
        }

        public bool Decrypt(string password)
        {
            if (this.IsDecrypted)
                return true;

            byte[] decrypted = Security.EncryptionProvider.Decrypt((byte[])Token.Clone(), this.EncryptionType, password);

            if (decrypted == null)
                return false;

            _decryptedToken = decrypted;
            return true;
        }

        public bool Encrypt(Security.EncryptionProvider.EncryptionType encType, string userPassword)
        {
            if (!this.IsDecrypted)
                return false;

            if (encType == Security.EncryptionProvider.EncryptionType.None)
            {
                _token = (byte[])_decryptedToken.Clone();
            }

            byte[] encryptedToken = Security.EncryptionProvider.Encrypt((byte[])_decryptedToken.Clone(), encType, userPassword);

            if (encryptedToken == null)
                return false;

            this.EncryptionType = encType;
            _token = encryptedToken;

            return true;
        }

        public string GetKey(long periodOffset)
        {
            if (!this.IsDecrypted)
                return null;

            long period = BlizzardAPI.ServerPeriod + periodOffset;

            byte[] periodBytes = BitConverter.GetBytes(period);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(periodBytes);
            }

            HMACSHA1 hmac = new HMACSHA1(this.DecryptedToken);
            byte[] digest = hmac.ComputeHash(periodBytes);

            int offset = digest[digest.Length - 1] & 0xf;

            byte[] keyBytes = new byte[4];
            Array.Copy(digest, offset, keyBytes, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(keyBytes);
            }

            UInt32 intKey = BitConverter.ToUInt32(keyBytes, 0) & 0x7fffffff; // Prevent last byte from comtaminating the modulo

            return (intKey % 100000000).ToString("00000000");
        }

        private void Initialize(string name, string serial, byte[] token, bool? supportsRestore, Security.EncryptionProvider.EncryptionType encryptionType)
        {
            if (!IsValidSerial(serial))
                throw new InvalidOperationException("Serial is invalid");

            this.Name = name;
            this.Serial = serial;
            this.Token = (byte[])token.Clone();

            this.EncryptionType = encryptionType;

            if (supportsRestore.HasValue == false)
            {
                try
                {
                    Authenticator auth = BlizzardAPI.RestoreAuthenticator(this.Serial, this.RestoreCode);

                    this.IsRestoreSupported = !(auth == null);
                }
                catch (Exception)
                {
                    this.IsRestoreSupported = false;
                }
            }
            else
            {
                this.IsRestoreSupported = (bool)supportsRestore;
            }
        }

        public void ToFile(string filePath)
        {
            using (BinaryWriter binWriter = new BinaryWriter(File.Create(filePath)))
            {
                binWriter.Write("WINBMAEXP".ToCharArray());
                binWriter.Write(2);

                if (this.Name != null)
                    binWriter.Write(this.Name);
                else
                    binWriter.Write("");

                binWriter.Write(this.Serial);
                binWriter.Write(this.IsRestoreSupported);
                binWriter.Write((byte)this.EncryptionType);
                binWriter.Write(this.Token.Length);
                binWriter.Write(this.Token);
            }
        }
    }
}