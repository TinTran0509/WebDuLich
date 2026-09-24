using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class CategoryModel : CategoryTran
    { 
        public string LangName { get; set; }
        public int Ordering { get; set; }
        public int TotalCount { get; set; }
    }
}
