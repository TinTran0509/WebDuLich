using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{  
    public class ItineraryModel
    {
        public int day { get; set; }
        public Title title { get; set; }
        public Content content { get; set; }
    }

    public class Title
    {
        public string en { get; set; }
        public string es { get; set; }
    }

    public class Content
    {
        public string en { get; set; }
        public string es { get; set; }
    }

}