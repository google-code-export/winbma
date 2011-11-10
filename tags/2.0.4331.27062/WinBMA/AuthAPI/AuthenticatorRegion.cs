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

namespace WinBMA.AuthAPI
{
    public class AuthenticatorRegion
    {
        public string RegionIdentifier
        {
            get
            {
                if (this.RegionType == RegionCode.NorthAmerica)
                {
                    return "US";
                }
                else
                {
                    return "EU";
                }
            }
        }

        private RegionCode _regionType;

        public RegionCode RegionType
        {
            get
            {
                return _regionType;
            }
            private set
            {
                _regionType = value;
            }
        }

        private AuthenticatorRegion() { }

        public static AuthenticatorRegion Factory(string regionType)
        {
            regionType = regionType.ToUpperInvariant();

            if (regionType == "US")
                return Factory(RegionCode.NorthAmerica);

            if (regionType == "EU")
                return Factory(RegionCode.Europe);

            return null;
        }

        public static AuthenticatorRegion Factory(RegionCode regionType)
        {
            if (regionType == RegionCode.NorthAmerica || regionType == RegionCode.Europe)
            {
                AuthenticatorRegion region = new AuthenticatorRegion();
                region.RegionType = regionType;

                return region;
            }

            return null;
        }

        public enum RegionCode
        {
            NorthAmerica = 1,
            Europe = 2
        }
    }
}