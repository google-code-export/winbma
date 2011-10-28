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
        private static byte[] ENROLL_RSA_EXPONENT = { 0x01, 0x01 };

        private static byte[] ENROLL_RSA_MODULUS = {0x95, 0x5e, 0x4b, 0xd9, 0x89, 0xf3, 0x91, 0x7d,
                                                    0x2f, 0x15, 0x54, 0x4a, 0x7e, 0x05, 0x04, 0xeb,
                                                    0x9d, 0x7b, 0xb6, 0x6b, 0x6f, 0x8a, 0x2f, 0xe4,
                                                    0x70, 0xe4, 0x53, 0xc7, 0x79, 0x20, 0x0e, 0x5e,
                                                    0x3a, 0xd2, 0xe4, 0x3a, 0x02, 0xd0, 0x6c, 0x4a,
                                                    0xdb, 0xd8, 0xd3, 0x28, 0xf1, 0xa4, 0x26, 0xb8,
                                                    0x36, 0x58, 0xe8, 0x8b, 0xfd, 0x94, 0x9b, 0x2a,
                                                    0xf4, 0xea, 0xf3, 0x00, 0x54, 0x67, 0x3a, 0x14,
                                                    0x19, 0xa2, 0x50, 0xfa, 0x4c, 0xc1, 0x27, 0x8d,
                                                    0x12, 0x85, 0x5b, 0x5b, 0x25, 0x81, 0x8d, 0x16,
                                                    0x2c, 0x6e, 0x6e, 0xe2, 0xab, 0x4a, 0x35, 0x0d,
                                                    0x40, 0x1d, 0x78, 0xf6, 0xdd, 0xb9, 0x97, 0x11,
                                                    0xe7, 0x26, 0x26, 0xb4, 0x8b, 0xd8, 0xb5, 0xb0,
                                                    0xb7, 0xf3, 0xac, 0xf9, 0xea, 0x3c, 0x9e, 0x00,
                                                    0x05, 0xfe, 0xe5, 0x9e, 0x19, 0x13, 0x6c, 0xdb,
                                                    0x7c, 0x83, 0xf2, 0xab, 0x8b, 0x0a, 0x2a, 0x99};

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
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters { Exponent = ENROLL_RSA_EXPONENT, Modulus = ENROLL_RSA_MODULUS });
            return rsa.Encrypt(payload, false);
        }

        private static byte[] GetRandomBytes(int numberOfBytes)
        {
            if (rng == null)
                rng = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[numberOfBytes];

            rng.GetBytes(buffer);

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

                plainTextStream.Write(Encoding.UTF8.GetBytes(UserAgent), 0, UserAgent.Length);

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

            // Token
            byte[] token = new byte[20];
            Array.Copy(responseData, 25, token, 0, 20);
            for (int i = xorRequestKey.Length - 1; i >= 0; i--)
            {
                token[i] ^= xorRequestKey[i];
            }

            // Serial
            string serial = Encoding.Default.GetString(responseData, 8, 17);

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