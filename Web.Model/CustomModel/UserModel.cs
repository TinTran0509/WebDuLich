using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class UserModel : tbl_User
    { 
        public string Description { get; set; }
        public string LangName { get; set; }
        public string Flag { get; set; }
    }
}
