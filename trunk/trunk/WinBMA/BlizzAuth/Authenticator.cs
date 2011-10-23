using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace WinBMA.BlizzAuth
{
    public class Authenticator
    {
        public Authenticator(string serial, string token)
        {
            if (!IsValidSerial(serial))
                throw new ArgumentException("Serial is invalid");

            if (!IsValidToken(token))
                throw new ArgumentException("Token is invalid");

            _serial = serial.ToUpper();
            _token = token;
        }

        private string _serial;
        public string Serial
        {
            get
            {
                return _serial;
            }
        }

        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
        }

        private Region _region;
        public Region Region
        {
            get
            {
                if (_region == null)
                {
                    if (Serial.Substring(0, 2) == "EU")
                        _region = Region.Factory(Region.RegionType.EU);
                    else
                        _region = Region.Factory(Region.RegionType.US);
                }

                return _region;
            }
        }

        public long TimeSinceLastKeyChange
        {
            get
            {
                return Region.ServerTime % 30000L;
            }
        }

        public string GetKey()
        {
            return GetKey(0);
        }

        public string GetKey(long offset)
        {
            long currentIteration = Region.ServerTime / 30000 + 30000 * offset;

            return CalculateAuthKey(currentIteration);
        }

        private byte[] _tokenDigest;
        private byte[] TokenDigest
        {
            get
            {
                if (_tokenDigest == null)
                {
                    _tokenDigest = Helper.ConvertHexStringToBytes(Token);
                }

                return _tokenDigest;
            }
        }

        private string CalculateAuthKey(long currentTimeIteration)
        {
            byte[] time_bytes = new byte[8];
            time_bytes[0] = (byte)(int)(currentTimeIteration >> 56);
            time_bytes[1] = (byte)(int)(currentTimeIteration >> 48);
            time_bytes[2] = (byte)(int)(currentTimeIteration >> 40);
            time_bytes[3] = (byte)(int)(currentTimeIteration >> 32);
            time_bytes[4] = (byte)(int)(currentTimeIteration >> 24);
            time_bytes[5] = (byte)(int)(currentTimeIteration >> 16);
            time_bytes[6] = (byte)(int)(currentTimeIteration >> 8);
            time_bytes[7] = (byte)(int)(currentTimeIteration);

            HMACSHA1 hmac = new HMACSHA1(TokenDigest);

            byte[] hmacDigest = hmac.ComputeHash(time_bytes);

            int result = TailBytesToInt(hmacDigest) % ((int)100000000);

            return result.ToString("00000000");
        }

        private static int TailBytesToInt(byte[] input)
        {
            int i = input[input.Length - 1] & 0xf;
            return ((input[i] & 0x7f) << 24) + ((input[i + 1] & 0xff) << 16) + ((input[i + 2] & 0xff) << 8) + (input[i + 3] & 0xff);
        }

        #region "Authenticator Creation"
        public static Authenticator Create(Region.RegionType region)
        {
            return Create(Region.Factory(region));
        }

        public static Authenticator Create(Region region)
        {
            return region.CreateAuthenticator();
        }

        public static bool IsValidSerial(string serial)
        {
            if (serial.Length != 17) return false;

            Regex rxTest = new Regex("^(EU|US)-[0-9]{4}-[0-9]{4}-[0-9]{4}$", RegexOptions.IgnoreCase);
            return rxTest.IsMatch(serial);
        }

        public static bool IsValidToken(string token)
        {
            if (token.Length != 40) return false;

            Regex rxTest = new Regex("^[0-9a-f]{40}$", RegexOptions.IgnoreCase);
            return rxTest.IsMatch(token);
        }
        #endregion
    }
}
