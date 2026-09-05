using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Commons
{
    [Serializable]
    public class UserLogin
    {
        public long UserID { set; get; }
        public string Username { set; get; }
    }
}