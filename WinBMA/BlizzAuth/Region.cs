using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WinBMA.BlizzAuth
{
    public class Region
    {
        private static Region[] _regions;

        public static Region Factory(RegionType region)
        {
            switch(region)
            {
                case RegionType.US:
                case RegionType.EU:
                    break;

                default:
                    region = RegionType.US;
                    break;
            }

            if(_regions == null) {
                _regions = new Region[2];
            }

            int regionCode = (int)region;

            if (_regions[regionCode] == null)
            {
                _regions[regionCode] = new Region(region);
            }

            return _regions[regionCode];
        }


        public enum RegionType
        {
            US = 0,
            EU = 1
        }

        private Region(RegionType region)
        {
            _region = region;
        }

        private RegionType _region = RegionType.US;

        public string EnrollURL
        {
            get
            {
                if (RegionFlag == RegionType.EU)
                    return "http://m.eu.mobileservice.blizzard.com/enrollment/enroll.htm";

                return "http://m.us.mobileservice.blizzard.com/enrollment/enroll.htm";
            }
        }

        public string ServerTimeURL
        {
            get
            {
                if (RegionFlag == RegionType.EU)
                    return "http://m.eu.mobileservice.blizzard.com/enrollment/time.htm";

                return "http://m.us.mobileservice.blizzard.com/enrollment/time.htm";
            }
        }

        public string RegionString
        {
            get
            {
                if (RegionFlag == RegionType.EU)
                    return "EU";

                return "US";
            }
        }

        public RegionType RegionFlag
        {
            get
            {
                if (_region == RegionType.EU)
                    return RegionType.EU;

                return RegionType.US;
            }
        }

        public int RegionNumber
        {
            get
            {
                return (int)RegionFlag;
            }
        }

        private long _timeDrift = long.MinValue;
        public long TimeDrift
        {
            get
            {
                if (_timeDrift == long.MinValue)
                {
                    if (Settings.TimeDifference[RegionNumber] != long.MinValue)
                    {
                        _timeDrift = Settings.TimeDifference[RegionNumber];
                    }
                    else
                    {
                        ResyncServerTime();
                    }
                }

                return _timeDrift;
            }
        }

        public void ResyncServerTime()
        {
            HttpWebRequest request = Helper.CreateRequest(ServerTimeURL);

            byte[] byServerTime = new byte[8];
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    responseStream.Read(byServerTime, 0, 8);
                }
            }

            _timeDrift = Helper.ConvertBytesToLong(byServerTime) - Helper.UnixTime;
            Settings.TimeDifference[RegionNumber] = _timeDrift;
            Settings.TimeSinceLastSync[RegionNumber] = DateTime.Now;
        }

        public long ServerTime
        {
            get
            {
                return TimeDrift + Helper.UnixTime;
            }
        }

        public Authenticator CreateAuthenticator()
        {
            BlizzCrypt encrypt = new BlizzCrypt(this);
            byte[] data_out = encrypt.OTP;

            HttpWebRequest request = Helper.CreateRequest(EnrollURL);
            request.Method = "POST";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data_out, 0, data_out.Length);
            }

            byte[] byServerTime = new byte[8];
            byte[] byTokenAndSerial = new byte[37];
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    responseStream.Read(byServerTime, 0, 8);
                    _timeDrift = Helper.ConvertBytesToLong(byServerTime) - Helper.UnixTime;
                    Settings.TimeDifference[RegionNumber] = _timeDrift;
                    Settings.TimeSinceLastSync[RegionNumber] = DateTime.Now;
                    responseStream.Read(byTokenAndSerial, 0, 37);
                }
            }

            byTokenAndSerial = encrypt.EncryptDecrypt(byTokenAndSerial);

            byte[] byToken = new byte[20];
            Array.Copy(byTokenAndSerial, 0, byToken, 0, 20);
            string token = Helper.ConvertBytesToHexString(byToken);

            byte[] bySerial = new byte[17];
            Array.Copy(byTokenAndSerial, 20, bySerial, 0, 17);
            string serial = Encoding.ASCII.GetString(bySerial, 0, 17);


            return new Authenticator(serial, token);
        }
    }
}
