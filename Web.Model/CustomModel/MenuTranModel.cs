using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class MenuTranModel : MenuTran
    {
        public int ProductID { get; set; }
        public string Image { get; set; }
        public string LangName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
    }
}
