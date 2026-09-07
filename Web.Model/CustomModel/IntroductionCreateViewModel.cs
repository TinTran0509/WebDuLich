using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class IntroductionCreateViewModel
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; } 
        public List<IntroductionLanguageViewModel> Languages { get; set; }
    }
}
