using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Model.CustomModel
{
    public class WordLanguageViewModel
    {
        public int ID { get; set; }  
        public string Value { get; set; }
        public string LangName { get; set; }
        public string LangCode { get; set; }
    }
}
