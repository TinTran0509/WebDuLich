using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class ProductModel : ProductTran
    {
        public string Image { get; set; }
        public string LangName { get; set; }
        public int MenuID { get; set; }
        public int TotalCount { get; set; }
    }
}
