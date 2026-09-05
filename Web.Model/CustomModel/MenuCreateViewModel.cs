using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class MenuCreateViewModel
    { 
        public string Image { get; set; }
        public int Ordering { get; set; }
        public int MenuID { get; set; }
        public int ParentID { get; set; }
        public List<MenuLanguageViewModel> Languages { get; set; }
    }
}
