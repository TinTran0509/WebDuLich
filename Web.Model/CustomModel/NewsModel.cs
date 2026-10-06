using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.CustomModel
{
    public class NewsModel : NewsTran
    {
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
}
