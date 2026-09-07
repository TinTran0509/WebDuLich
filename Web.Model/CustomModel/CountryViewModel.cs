using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class CountryViewModel : Country
    {
        public string Name { get; set; }
        public string LangName { get; set; }
        public int CountryID { get; set; }
    }
}
