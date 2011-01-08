using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinBMA
{
    public class FriendlyAuth
    {
        public FriendlyAuth(BlizzAuth.Authenticator auth, string name = "")
        {
            _auth = auth;
            _friendlyName = name;
        }

        private BlizzAuth.Authenticator _auth;
        public BlizzAuth.Authenticator Authenticator
        {
            get
            {
                return _auth;
            }
            set
            {
                _auth = value;
            }
        }

        private string _friendlyName;
        public string FriendlyName
        {
            get
            {
                return _friendlyName;
            }
            set
            {
                _friendlyName = value;
            }
        }

        public override string ToString()
        {
            if (_friendlyName == string.Empty)
            {
                return _auth.Serial;
            }
            else
            {
                return _friendlyName;
            }
        }
    }
}
