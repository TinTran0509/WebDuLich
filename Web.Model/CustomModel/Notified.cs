using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class Notified
    {
        public EnumNotifield Value { get; set; }
        public string Messenger { get; set; }
    }

    public enum EnumNotifield
    {
        Success = 1,
        Error = 2,
        Warring = 3
    }
}
