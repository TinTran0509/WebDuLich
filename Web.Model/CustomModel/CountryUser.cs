using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class CountryUser : Country
    {
        public string UserAvatar { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
    }
}
