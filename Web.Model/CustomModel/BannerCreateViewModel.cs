using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class BannerCreateViewModel
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public int MenuID { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<BannerLanguageViewModel> Languages { get; set; }
    }
}
