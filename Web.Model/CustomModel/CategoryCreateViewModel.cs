using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class CategoryCreateViewModel
    {
        public int ID { get; set; } 
        public int Ordering { get; set; } 
        public List<CategoryLanguageViewModel> Languages { get; set; }
    }
}
