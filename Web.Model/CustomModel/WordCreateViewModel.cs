using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class WordCreateViewModel
    {
        public int ID { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; } 
        public List<WordLanguageViewModel> Languages { get; set; }
    }
}
