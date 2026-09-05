using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class CountryCreateViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
        public bool Active { get; set; }
        public int Ordering { get; set; }
        public List<CountryLanguageViewModel> Languages { get; set; }
    }
}
