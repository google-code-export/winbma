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
using System.Net;
using System.Security.Cryptography;
using System.Text;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using BouncyCastleBigInteger = Org.BouncyCastle.Math.BigInteger;

namespace WinBMA.AuthAPI
{
    public static class BlizzardAPI
    {
        private const int ENROLL_RESPONSE_SIZE = 45;
        public const long HOTP_PERIOD_LENGTH = 30000;
        private const int RESPONSE_BUFFER_SIZE = 64;
        private const int RESTORE_RESPONSE_SIZE = 32;
        private const int RESTOREVALIDATE_RESPONSE_SIZE = 20;
        private const int SYNC_RESPONSE_SIZE = 8;
        private static string AUTHORITY = "http://mobile-service.blizzard.com";
        private static string ENDPOINT_ENROLL = "/enrollment/enroll2.htm";
        private static string ENDPOINT_RESTORE = "/enrollment/initiatePaperRestore.htm";
        private static string ENDPOINT_RESTOREVALIDATE = "/enrollment/validatePaperRestore.htm";
        private static string ENDPOINT_SYNC = "/enrollment/time.htm";

        private static string ENROLL_RSA_EXPONENT = "0101";
        private static string ENROLL_RSA_MODULUS = "955e4bd989f3917d2f15544a7e0504eb9d7bb66b6f8a2fe470e453c779200e5e" +
                                                   "3ad2e43a02d06c4adbd8d328f1a426b83658e88bfd949b2af4eaf30054673a14" +
                                                   "19a250fa4cc1278d12855b5b25818d162c6e6ee2ab4a350d401d78f6ddb99711" +
                                                   "e72626b48bd8b5b0b7f3acf9ea3c9e0005fee59e19136cdb7c83f2ab8b0a2a99";

        private static Int64 LocalUnixTime
        {
            get
            {
                return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            }
        }

        public static Int64 MillisecondsSincePeriod
        {
            get
            {
                return ServerTime % HOTP_PERIOD_LENGTH;
            }
        }

        private static RNGCryptoServiceProvider rng;

        public static Int64 ServerPeriod
        {
            get
            {
                return ServerTime / HOTP_PERIOD_LENGTH;
            }
        }

        public static Int64 ServerTime
        {
            get
            {
                return LocalUnixTime + Settings.SettingsDatabase.ServerTimeOffset;
            }
        }

        private static string userAgent;

        private static string UserAgent
        {
            get
            {
                if (userAgent == null)
                {
                    userAgent = "WinBMA/" + App.Version.ToString();
                }

                return userAgent;
            }
        }

        private static long CalculateOffset(byte[] serverTime)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(serverTime);
            }

            return BitConverter.ToInt64(serverTime, 0) - LocalUnixTime;
        }

        private static byte[] EncryptRSAPayload(byte[] payload)
        {
            //! NOTE: I am using BouncyCastle/RSAEngine due to the fact that RSACryptoServiceProvider will
            //        not let you encrypt without padding.

            RsaEngine rsaCryptoProvider = new RsaEngine();
            rsaCryptoProvider.Init(true, new RsaKeyParameters(false, new BouncyCastleBigInteger(ENROLL_RSA_MODULUS, 16), new BouncyCastleBigInteger(ENROLL_RSA_EXPONENT, 16)));
            return rsaCryptoProvider.ProcessBlock(payload, 0, payload.Length);
        }

        private static byte[] GetRandomBytes(int numberOfBytes, bool printableOnly = false)
        {
            if (rng == null)
                rng = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[numberOfBytes];

            rng.GetBytes(buffer);

            if (printableOnly)
            {
                for (int i = 0; i < numberOfBytes; i++)
                {
                    buffer[i] = (byte)((buffer[i] % 62) + 48); // Numeric

                    if (buffer[i] > 57)
                        buffer[i] += 7; // Go In Upper Case

                    if (buffer[i] > 90)
                        buffer[i] += 6; // Go In Lower Case
                }
            }

            return buffer;
        }

        private static HttpWebResponse HTTPGet(string endpoint)
        {
            HttpWebRequest request = HttpWebRequest.Create(AUTHORITY + endpoint) as HttpWebRequest;
            request.Timeout = 5000;
            request.UserAgent = UserAgent;
            request.Method = "GET";

            return (HttpWebResponse)request.GetResponse();
        }

        private static HttpWebResponse HTTPPost(string endpoint, byte[] postData)
        {
            HttpWebRequest request = HttpWebRequest.Create(AUTHORITY + endpoint) as HttpWebRequest;
            request.Timeout = 5000;
            request.UserAgent = UserAgent;
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = postData.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            return (HttpWebResponse)request.GetResponse();
        }

        public static Authenticator RequestAuthenticator()
        {
            return RequestAuthenticator(null);
        }

        public static Authenticator RequestAuthenticator(AuthenticatorRegion authRegion)
        {
            byte[] plainTextRequest;
            byte[] xorRequestKey = GetRandomBytes(20);

            using (MemoryStream plainTextStream = new MemoryStream())
            {
                plainTextStream.Write(xorRequestKey, 0, 20); // Write Randomized Padding

                // Force Blizzard to return an authenticator from a specific region if requested
                if (authRegion != null)
                {
                    string countryCode = (authRegion.RegionType == AuthenticatorRegion.RegionCode.NorthAmerica) ? "US" : "FR";
                    plainTextStream.Write(Encoding.UTF8.GetBytes(countryCode), 0, 2);
                }
                else
                {
                    plainTextStream.Write(new byte[] { 0x0, 0x0 }, 0, 2);
                }

                plainTextStream.Write(GetRandomBytes(16, true), 0, 16);

                plainTextRequest = plainTextStream.ToArray();
            }

            byte[] encryptedRequest = EncryptRSAPayload(plainTextRequest);
            byte[] responseData = null;

            try
            {
                using (HttpWebResponse response = HTTPPost(ENDPOINT_ENROLL, encryptedRequest))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (Stream binaryStream = response.GetResponseStream())
                        {
                            byte[] temp = new byte[RESPONSE_BUFFER_SIZE];

                            int read;
                            while ((read = binaryStream.Read(temp, 0, RESPONSE_BUFFER_SIZE)) != 0)
                            {
                                memStream.Write(temp, 0, read);
                            }
                            responseData = memStream.ToArray();

                            if (responseData.Length != ENROLL_RESPONSE_SIZE)
                            {
                                throw new InvalidBlizzardAPIResponse(string.Format("Invalid response size (expected {0} got {1})", ENROLL_RESPONSE_SIZE, responseData.Length));
                            }
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                HttpWebResponse response = webEx.Response as HttpWebResponse;

                throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
            }

            // Server Time Offset
            byte[] serverTime = new byte[8];
            Array.Copy(responseData, serverTime, 8);

            Settings.SettingsDatabase.ServerTimeOffset = CalculateOffset(serverTime);

            // Serial
            string serial = Encoding.Default.GetString(responseData, 8, 17);

            // Token
            byte[] token = new byte[20];
            Array.Copy(responseData, 25, token, 0, 20);

            for (int i = xorRequestKey.Length - 1; i >= 0; i--)
            {
                token[i] ^= xorRequestKey[i];
            }

            return new Authenticator(serial, token);
        }

        public static Authenticator RestoreAuthenticator(string serial, string restoreCode)
        {
            if (!Authenticator.IsValidSerial(serial))
                return null;

            if (restoreCode.Length != 10)
                return null;

            byte[] serialBytes = Encoding.UTF8.GetBytes(serial.ToUpperInvariant().Replace("-", ""));

            byte[] responseData = null;

            try
            {
                using (HttpWebResponse response = HTTPPost(ENDPOINT_RESTORE, serialBytes))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (Stream binaryStream = response.GetResponseStream())
                        {
                            byte[] temp = new byte[RESPONSE_BUFFER_SIZE];

                            int read;
                            while ((read = binaryStream.Read(temp, 0, RESPONSE_BUFFER_SIZE)) != 0)
                            {
                                memStream.Write(temp, 0, read);
                            }
                            responseData = memStream.ToArray();

                            if (responseData.Length != RESTORE_RESPONSE_SIZE)
                            {
                                throw new InvalidBlizzardAPIResponse(string.Format("Invalid response size (expected {0} got {1})", RESTORE_RESPONSE_SIZE, responseData.Length));
                            }
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                HttpWebResponse response = webEx.Response as HttpWebResponse;

                throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
            }

            byte[] restoreCodeBytes = Security.RestoreCodeProvider.ToBytes(restoreCode);

            if (restoreCodeBytes == null)
                return null;

            byte[] challengeSignature = new byte[serialBytes.Length + responseData.Length];
            Array.Copy(serialBytes, challengeSignature, serialBytes.Length);
            Array.Copy(responseData, 0, challengeSignature, serialBytes.Length, responseData.Length);

            HMACSHA1 hmac = new HMACSHA1(restoreCodeBytes);
            byte[] challengeResponse = hmac.ComputeHash(challengeSignature);

            byte[] plainTextRequest;
            byte[] xorRequestKey = GetRandomBytes(20);

            using (MemoryStream plainTextStream = new MemoryStream())
            {
                plainTextStream.Write(challengeResponse, 0, challengeResponse.Length);
                plainTextStream.Write(xorRequestKey, 0, 20);

                plainTextRequest = plainTextStream.ToArray();
            }

            byte[] encryptedRequest = EncryptRSAPayload(plainTextRequest);

            byte[] requestData = new byte[serialBytes.Length + encryptedRequest.Length];
            Array.Copy(serialBytes, requestData, serialBytes.Length);
            Array.Copy(encryptedRequest, 0, requestData, serialBytes.Length, encryptedRequest.Length);

            responseData = null;
            try
            {
                using (HttpWebResponse response = HTTPPost(ENDPOINT_RESTOREVALIDATE, requestData))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (Stream binaryStream = response.GetResponseStream())
                        {
                            byte[] temp = new byte[RESPONSE_BUFFER_SIZE];

                            int read;
                            while ((read = binaryStream.Read(temp, 0, RESPONSE_BUFFER_SIZE)) != 0)
                            {
                                memStream.Write(temp, 0, read);
                            }
                            responseData = memStream.ToArray();

                            if (responseData.Length != RESTOREVALIDATE_RESPONSE_SIZE)
                            {
                                throw new InvalidBlizzardAPIResponse(string.Format("Invalid response size (expected {0} got {1})", RESTOREVALIDATE_RESPONSE_SIZE, responseData.Length));
                            }
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                HttpWebResponse response = webEx.Response as HttpWebResponse;

                throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
            }

            for (int i = xorRequestKey.Length - 1; i >= 0; i--)
            {
                responseData[i] ^= xorRequestKey[i];
            }

            return new Authenticator(serial, responseData);
        }

        public static void SyncClock()
        {
            byte[] responseData = null;

            try
            {
                using (HttpWebResponse response = HTTPGet(ENDPOINT_SYNC))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (Stream binaryStream = response.GetResponseStream())
                        {
                            byte[] temp = new byte[RESPONSE_BUFFER_SIZE];

                            int read;
                            while ((read = binaryStream.Read(temp, 0, RESPONSE_BUFFER_SIZE)) != 0)
                            {
                                memStream.Write(temp, 0, read);
                            }
                            responseData = memStream.ToArray();

                            if (responseData.Length != SYNC_RESPONSE_SIZE)
                            {
                                throw new InvalidBlizzardAPIResponse(string.Format("Invalid response size (expected {0} got {1})", SYNC_RESPONSE_SIZE, responseData.Length));
                            }
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                HttpWebResponse response = webEx.Response as HttpWebResponse;
                throw new InvalidBlizzardAPIResponse(String.Format("HTTP {0}: {1}", response.StatusCode, response.StatusDescription));
            }

            Settings.SettingsDatabase.ServerTimeOffset = CalculateOffset(responseData);
        }
    }
}