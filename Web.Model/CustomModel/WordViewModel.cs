using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class WordViewModel : Word
    { 
        public string Value { get; set; }
        public int WordID { get; set; }
        public string LangName { get; set; }
    }
}
