using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class LocationCreateViewModel
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public int CountryID { get; set; }
        public List<LocationLanguageViewModel> Languages { get; set; }
    }
}
