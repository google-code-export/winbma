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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WinBMA.AuthAPI.Security
{
    public static class EncryptionProvider
    {
        private static byte[] WINBMA_SECRET = {0x77, 0xfd, 0x14, 0xda, 0x3b, 0xc9, 0x49, 0x25,
                                               0x21, 0x5f, 0x2c, 0xf3, 0x84, 0xb3, 0x01, 0xff,
                                               0x46, 0x9d, 0x2e, 0x11, 0x0e, 0x74, 0x0b, 0x7a,
                                               0x40, 0x28, 0x7f, 0x39, 0x36, 0x42, 0x19, 0xa4};

        private static byte[] AESDecrypt(byte[] cipherText, string userPass)
        {
            if (cipherText.Length < 17)
                throw new CryptographicException("Cipher Bytes too small");

            byte[] derivedKey = PBKDF2Derive(userPass);

            byte[] iv = new byte[16];
            Array.Copy(cipherText, iv, 16);

            byte[] aesEncrypted = new byte[cipherText.Length - 16];
            Array.Copy(cipherText, 16, aesEncrypted, 0, aesEncrypted.Length);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = derivedKey;
            aes.IV = iv;

            ICryptoTransform cipher = aes.CreateDecryptor();
            return cipher.TransformFinalBlock(aesEncrypted, 0, aesEncrypted.Length);
        }

        private static byte[] AESEncrypt(byte[] plainText, string userPass)
        {
            byte[] derivedKey = PBKDF2Derive(userPass);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = derivedKey;
            aes.GenerateIV();

            ICryptoTransform cipher = aes.CreateEncryptor();

            byte[] aesEncrypted = cipher.TransformFinalBlock(plainText, 0, plainText.Length);
            byte[] encrypted = new byte[aesEncrypted.Length + 16];

            Array.Copy(aes.IV, encrypted, 16);
            Array.Copy(aesEncrypted, 0, encrypted, 16, aesEncrypted.Length);

            return encrypted;
        }

        public static byte[] Decrypt(byte[] encrypted, EncryptionType encType, string userPassword)
        {
            if (encType == EncryptionType.None)
                return encrypted;

            // Decrypt second pass (AES/PBKDF2)
            byte[] secondPassDecrypt;

            if (encType.HasFlag(EncryptionType.Password))
            {
                try
                {
                    secondPassDecrypt = AESDecrypt(encrypted, userPassword);
                }
                catch (Exception) { return null; }
            }
            else
            {
                secondPassDecrypt = encrypted;
            }

            // Decrypt first pass (Machine/User Lock)
            byte[] decrypted;

            if (encType.HasFlag(EncryptionType.LocalUser) || encType.HasFlag(EncryptionType.LocalMachine))
            {
                try
                {
                    decrypted = MachineDecrypt(secondPassDecrypt, encType);
                }
                catch (Exception) { return null; }
            }
            else
            {
                decrypted = secondPassDecrypt;
            }

            // Verify token signature
            if (decrypted.Length != 30)
                return null;

            byte[] token = new byte[20];
            Array.Copy(decrypted, token, 20);

            byte[] signature = new byte[10];
            Array.Copy(decrypted, 20, signature, 0, 10);

            byte[] expectedSignature = SignToken(token);

            if (expectedSignature.SequenceEqual(signature))
                return token;

            return null;
        }

        public static byte[] Encrypt(byte[] token, EncryptionType encType, string userPassword)
        {
            if (encType == EncryptionType.None)
                return token;

            // Sign Token (for verification at decryption time)
            byte[] signature = SignToken(token);
            byte[] plainText = new byte[30];

            Array.Copy(token, plainText, 20);
            Array.Copy(signature, 0, plainText, 20, 10);

            // Encrypt first pass (Machine/User Lock)
            byte[] firstPassEncrypted;

            if (encType.HasFlag(EncryptionType.LocalUser) || encType.HasFlag(EncryptionType.LocalMachine))
            {
                firstPassEncrypted = MachineEncrypt(plainText, encType);
            }
            else
            {
                firstPassEncrypted = plainText;
            }

            // Encrypt second pass (AES/PBKDF2)
            byte[] encrypted = null;

            if (encType.HasFlag(EncryptionType.Password))
            {
                encrypted = AESEncrypt(firstPassEncrypted, userPassword);
            }
            else
            {
                return firstPassEncrypted;
            }

            return encrypted;
        }

        private static byte[] MachineDecrypt(byte[] cipherText, EncryptionType encType)
        {
            DataProtectionScope scope = (encType.HasFlag(EncryptionType.LocalUser)) ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine;
            return ProtectedData.Unprotect(cipherText, WINBMA_SECRET, scope);
        }

        private static byte[] MachineEncrypt(byte[] plainText, EncryptionType encType)
        {
            DataProtectionScope scope = (encType.HasFlag(EncryptionType.LocalUser)) ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine;
            return ProtectedData.Protect(plainText, WINBMA_SECRET, scope);
        }

        private static byte[] PBKDF2Derive(string userPassword)
        {
            return PBKDF2Derive(Encoding.UTF8.GetBytes(userPassword), 32, 1000);
        }

        private static byte[] PBKDF2Derive(byte[] input, int keyLength, int iterationCount)
        {
            Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(input, WINBMA_SECRET, iterationCount);
            return derive.GetBytes(keyLength);
        }

        private static byte[] SignToken(byte[] token)
        {
            return PBKDF2Derive(token, 10, 128);
        }

        [Flags]
        public enum EncryptionType
        {
            None = 0,
            Password = 1,
            LocalUser = 2,
            LocalMachine = 4
        }
    }
}