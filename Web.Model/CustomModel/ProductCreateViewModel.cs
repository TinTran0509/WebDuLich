using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class ProductCreateViewModel
    {
        public string ProductCode { get; set; }
        public int ID { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public int MenuID { get; set; }
        public int Type { get; set; }
        public double Price { get; set; }
        public int Size { get; set; }
        public int DayNumber { get; set; }
        public int CountryID { get; set; }
        public List<int> LocationID { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ProductLanguageViewModel> Languages { get; set; }
    }
}
