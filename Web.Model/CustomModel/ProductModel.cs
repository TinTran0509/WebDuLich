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
        public string ProductCode { get; set; }
        public string LangName { get; set; }
        public int MenuID { get; set; }
        public int DayNumber { get; set; }
        public int NumberStar { get; set; }
        public int Type { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public int TotalCount { get; set; }
        public string Countries { get; set; }
        public string LocationID { get; set; }
        public string Locations { get; set; }
    }
}
