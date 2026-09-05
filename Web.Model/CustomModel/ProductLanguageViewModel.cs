using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Model.CustomModel
{
    public class ProductLanguageViewModel
    {
        public int ID { get; set; }

        public string LangCode { get; set; }

        public string LangName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Contents { get; set; } 
    }
}
